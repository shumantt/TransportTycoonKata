// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open TycoonExersizes3
open TycoonExersizes3.Types

// Define a function to construct a message to print
let from whom = sprintf "from %s" whom

let private parseInputCargoes (cargoes: string) =
    cargoes
    |> Seq.indexed
    |> Seq.map (fun (i, c) ->
        let targetLocation =
            match c with
            | 'A' -> Sink Location.A
            | 'B' -> Sink Location.B
            | _ -> failwith "unknownSink"

        { id = CargoId (i + 1)
          destination = targetLocation })
    |> Seq.toList

let private buildWarehouse cargoes location =
    let warehouse = { location = location; storage = [] }
    Warehouse.unloadCargo warehouse cargoes (TimePoint 0)

let private vehicles =
    [ { id = VehicleId 1
        vehicleType = TRUCK
        status = { arriveTime = TimePoint 0 } }
      { id = VehicleId 2
        vehicleType = TRUCK
        status = { arriveTime = TimePoint 0 } }
      { id = VehicleId 3
        vehicleType = SHIP
        status = { arriveTime = TimePoint 0 } } ]

let private routes =
    [ { segments =
            [ { startPoint = Location.FACTORY
                endPoint = Location.B
                coveredBy = TRUCK
                length = Duration 5 } ] }
      { segments =
            [ { startPoint = Location.FACTORY
                endPoint = Location.PORT
                coveredBy = TRUCK
                length = Duration 1 }
              { startPoint = Location.PORT
                endPoint = Location.A
                coveredBy = SHIP
                length = Duration 6 } ] } ]


[<EntryPoint>]
let main argv =
    let cargoes = parseInputCargoes argv.[0]

    let warehouses =
        [ buildWarehouse cargoes Location.FACTORY
          buildWarehouse [] Location.PORT ]

    let initialState = {
        warehouses = warehouses
        vehicles = vehicles
        deliveredItems = []
    }
    
    let deliveryResult = Delivery.deliver routes initialState
    deliveryResult.events
    |> Events.logEvents

    0 // return an integer exit code
