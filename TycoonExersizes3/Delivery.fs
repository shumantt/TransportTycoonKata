module TycoonExersizes3.Delivery
open System
open System.Collections.Generic
open TycoonExersizes3.Types

let private getRoute routes targetPoint =
    routes
        |> List.find (fun r -> r |> Route.targetPoint = targetPoint)

let private getFirstFreeVehicle (vehicles: Vehicle list) vehicleType =
    vehicles
        |> List.filter (fun v -> v.vehicleType = vehicleType)
        |> List.sortBy (fun v -> v.status.arriveTime, v.id)
        |> List.head
        
let private tryGetTargetWarehouse (warehouses: Warehouse list) targetLocation =
    warehouses |> List.tryFind (fun x -> x.location = targetLocation)

let private deliverOnSegment unloadCargoesToTargetWarehouse vehicle segment cargoes (cargoesReadyTime: TimePoint) =
    let unloadingTime = Vehicle.unLoadingTime vehicle
    let startLoading = TimePoint (Math.Max(cargoesReadyTime.value, vehicle.status.arriveTime.value))
    let finishLoading = startLoading + unloadingTime
    let arriveToDestination = finishLoading + segment.length
    let finishUnloading = arriveToDestination + unloadingTime
    
    let (newTargetWarehouseState, deliveredItems) = unloadCargoesToTargetWarehouse finishUnloading
    
    let comebackTime = finishUnloading + segment.length
    let newVehicleState = { vehicle with status = { arriveTime = comebackTime }  }
    let events : VehicleEvent list = [
        Events.createLoadEvent startLoading vehicle segment.startPoint [] unloadingTime ;
        Events.createDepartEvent finishLoading vehicle segment.startPoint cargoes segment.endPoint ;
        Events.createArriveEvent arriveToDestination vehicle segment.endPoint cargoes ;
        Events.createUnloadEvent arriveToDestination vehicle segment.startPoint cargoes unloadingTime ;
        Events.createDepartEvent finishUnloading vehicle segment.endPoint [] segment.startPoint ;
        Events.createArriveEvent comebackTime vehicle segment.startPoint []
    ]
    newVehicleState, newTargetWarehouseState, deliveredItems, events

let private unloadCargo cargoes targetWarehouse atTime =
    match targetWarehouse with
    | Some warehouse ->
        let newTargetWarehouseState = Warehouse.unloadCargo warehouse cargoes atTime
        Some newTargetWarehouseState, []
    | None -> None, cargoes |> List.map(fun c -> { cargo = c
                                                   time = atTime })
    
let private updateDeliveryState currentState (newSourceWarehouseState: Warehouse) newTargetWarehouseState newVehicleState deliveredItems =
    let newWarehouses = 
        match newTargetWarehouseState with
        | Some newTarget -> [ newSourceWarehouseState; newTarget ]
        | None ->
            let notUpdatedWarehouses =
                currentState.warehouses
                |> List.filter (fun w -> w.location <> newSourceWarehouseState.location)
            newSourceWarehouseState :: notUpdatedWarehouses
    
    let newVehiclesState =
        let notUpdatedVehicles = currentState.vehicles |> List.filter (fun v -> v.id <> newVehicleState.id)
        newVehicleState :: notUpdatedVehicles
    
    let deliveredItems = currentState.deliveredItems @ deliveredItems
    
    { warehouses = newWarehouses
      vehicles = newVehiclesState
      deliveredItems = deliveredItems }

let rec private processDelivery routes state =
    let processWarehouse warehouse state =
        let rec processWarehouse warehouse state warehouseEvents =
            match warehouse.storage with
            | [] -> (state, warehouseEvents)
            | _ ->
                let segment =
                    Warehouse.getNextItemDestination warehouse
                    |> getRoute routes
                    |> Route.nextSegment warehouse.location
                
                let vehicle = getFirstFreeVehicle state.vehicles segment.coveredBy
                let vehicleCapacity = Vehicle.capacity vehicle
                
                let newSourceWarehouseState, (readyTime: TimePoint), cargoesToLoad =
                    Warehouse.loadItemsFromWarehouse warehouse vehicle.status.arriveTime vehicleCapacity
                    
                let unloadCargo =
                    tryGetTargetWarehouse state.warehouses segment.endPoint
                    |> unloadCargo cargoesToLoad         
                
                let newVehicleState, newTargetWarehouseState, deliveredItems, vehicleEvents =
                    deliverOnSegment unloadCargo vehicle segment cargoesToLoad readyTime
                
                let newState = updateDeliveryState state newSourceWarehouseState newTargetWarehouseState newVehicleState deliveredItems
                processWarehouse newSourceWarehouseState newState (warehouseEvents @ vehicleEvents)
        processWarehouse warehouse state []
    
    let rec processWarehouses warehouses state eventsAcc =
        match warehouses with
        | [] -> state, eventsAcc
        | warehouse :: leftWarehouses ->
            let stateAfterWarehouseProcess, events = processWarehouse warehouse state
            let actualLeftWarehouses =
                stateAfterWarehouseProcess.warehouses
                |> List.filter (fun x -> leftWarehouses |> Seq.map (fun x -> x.location) |> Seq.contains x.location )
            
            processWarehouses actualLeftWarehouses stateAfterWarehouseProcess (eventsAcc @ events)
            
    processWarehouses state.warehouses state []
            
let private getDeliveryDuration (item: DeliveredItem) =
    let (TimePoint deliveredAt) = item.time
    Duration deliveredAt

let deliver: DeliveryProcess =
    fun routes initialState  ->
        let (finalState, events) = processDelivery routes initialState
        let deliveryDuration = finalState.deliveredItems
                             |> List.maxBy (fun item -> item.time)
                             |> getDeliveryDuration
        { time = deliveryDuration
          events = events }
