module TycoonExersizes3.Events
open System
open System.Text.Json
open TycoonExersizes3.Types

let private (|EVENT|) (vehicleEvent : VehicleEvent) =
     match vehicleEvent with
     | ARRIVE arrive -> arrive
     | DEPART depart -> depart
     | LOAD load -> load
     | UNLOAD unload -> unload


let private createBaseEvent atTime vehicle location (cargoes: Cargo list) duration destination =
    { time = atTime
      vehicleId = vehicle.id
      vehicleType = vehicle.vehicleType
      location = location
      destination = destination
      cargo = cargoes
      duration = duration }
    
let private convertToCargoLog (cargoes: Cargo list) =
    cargoes
    |> List.map(fun c ->
        let (Sink location) = c.destination
        { cargo_id = c.id.value
          destination =  location.getLocationName()
          origin = "FACTORY" })
    
let private convertToEventLog (event:VehicleEvent) =
    let baseEvent = match event with | EVENT baseEvent -> baseEvent
    { event = event.getEventName()
      time = baseEvent.time.value
      transport_id = baseEvent.vehicleId.value
      kind = baseEvent.vehicleType.getTypeName()
      location = baseEvent.location.getLocationName()
      destination = match baseEvent.destination with
                    | Some x -> x.getLocationName()
                    | None -> null
      cargo = convertToCargoLog baseEvent.cargo
      duration = match baseEvent.duration with
                 | Some x -> Nullable x.value
                 | _ -> Nullable() }
    

let createArriveEvent atTime vehicle location (cargoes: Cargo list) =
    ARRIVE (createBaseEvent atTime vehicle location (cargoes: Cargo list) None None)
    
let createDepartEvent atTime vehicle location (cargoes: Cargo list) destination =
    DEPART (createBaseEvent atTime vehicle location (cargoes: Cargo list) None (Some destination))
   
let createLoadEvent atTime vehicle location (cargoes: Cargo list) duration =
    LOAD (createBaseEvent atTime vehicle location (cargoes: Cargo list) (Some duration) None)
    
let createUnloadEvent atTime vehicle location (cargoes: Cargo list) duration =
    UNLOAD (createBaseEvent atTime vehicle location (cargoes: Cargo list) (Some duration) None)
    
    
let logEvents events =
    events
    |> List.map convertToEventLog
    |> List.map JsonSerializer.Serialize
    |> List.iter Console.WriteLine