module TycoonExersizes3.Types

open System
open Microsoft.FSharp.Reflection

type Location =
    | FACTORY
    | PORT
    | A
    | B
    member x.getLocationName() =
        match FSharpValue.GetUnionFields(x, x.GetType()) with
        | case, _ -> case.Name

type VehicleType =
    | TRUCK
    | SHIP
    member x.getTypeName() =
        match FSharpValue.GetUnionFields(x, x.GetType()) with
        | case, _ -> case.Name


type Duration =
    | Duration of int
    member x.value =
        let (Duration underlyingValue) = x
        underlyingValue

type TimePoint =
    | TimePoint of int
    static member (+)(TimePoint a, Duration b) = TimePoint(a + b)

    member x.value =
        let (TimePoint underlyingValue) = x
        underlyingValue


type VehicleId =
    | VehicleId of int
    member x.value =
        let (VehicleId underlyingValue) = x
        underlyingValue

type CargoId =
    | CargoId of int
    member x.value =
        let (CargoId underlyingValue) = x
        underlyingValue


type Sink = Sink of Location

type Cargo = { id: CargoId; destination: Sink }

type WarehouseStorageItem = { cargo: Cargo; unloadTime: TimePoint }

type Warehouse =
    { location: Location
      storage: WarehouseStorageItem list }

type VehicleStatus = { arriveTime: TimePoint }

type Vehicle =
    { id: VehicleId
      vehicleType: VehicleType
      status: VehicleStatus }

type RouteSegment =
    { startPoint: Location
      endPoint: Location
      length: Duration
      coveredBy: VehicleType }

type Route = { segments: RouteSegment list }

type DeliveredItem = { cargo: Cargo; time: TimePoint }

type DeliveryState =
    { warehouses: Warehouse list
      vehicles: Vehicle list
      deliveredItems: DeliveredItem list }

type BaseEvent =
    { time: TimePoint
      vehicleId: VehicleId
      vehicleType: VehicleType
      location: Location
      destination: Location option
      cargo: Cargo list
      duration: Duration option }

type VehicleEvent =
    | ARRIVE of BaseEvent
    | DEPART of BaseEvent
    | LOAD of BaseEvent
    | UNLOAD of BaseEvent
    member x.getEventName() =
        match FSharpValue.GetUnionFields(x, x.GetType()) with
        | case, _ -> case.Name

type DeliveryResult =
    { time: Duration
      events: VehicleEvent list }

type DeliveryProcess = Route list -> DeliveryState -> DeliveryResult


type CargoLog =
    { cargo_id: int
      destination: string
      origin: string }

type EventLog =
    { event: string
      time: int
      transport_id: int
      kind: string
      location: string
      destination: string
      cargo: CargoLog list
      duration: Nullable<int> }
