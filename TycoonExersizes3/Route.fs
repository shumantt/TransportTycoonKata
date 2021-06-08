module TycoonExersizes3.Route
open TycoonExersizes3.Types
    
let nextSegment startPoint route =
    route.segments
    |> List.find (fun segment -> segment.startPoint = startPoint)
    
let targetPoint route =
    route.segments
    |> List.last
    |> fun s -> Sink s.endPoint