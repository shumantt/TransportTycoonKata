module TycoonExersizes3.Vehicle

open TycoonExersizes3.Types

let capacity (vehicle: Vehicle) =
    match vehicle.vehicleType with
    | TRUCK _ -> 1
    | SHIP _ -> 4
    
let unLoadingTime (vehicle: Vehicle) =
    match vehicle.vehicleType with
    | TRUCK _ -> Duration 0
    | SHIP _ -> Duration 1