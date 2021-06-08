module TycoonExersizes3.Warehouse

open TycoonExersizes3.Types

let unloadCargo (warehouse: Warehouse) cargoes atTime =
    let storageItems =
        cargoes
        |> List.map (fun c -> { cargo = c
                                unloadTime = atTime })
    { warehouse with storage = warehouse.storage @ storageItems }

let private canLoadCargo targetPoint
                         atTime
                         maxCount
                         (loadedItems: WarehouseStorageItem list)
                         (storageItem: WarehouseStorageItem)
                         =
    let cargo = storageItem.cargo
    let itemIsInStockAtTime = storageItem.unloadTime <= atTime

    let loadingPackIsNotReady =
        match loadedItems |> List.tryHead with
        | Some lastLoaded -> lastLoaded.unloadTime = storageItem.unloadTime
        | None -> true

    loadedItems.Length < maxCount
    && cargo.destination = targetPoint
    && (itemIsInStockAtTime || loadingPackIsNotReady)


let private splitItems toTake items =
    let rec splitInternal items itemsToTake =
        match items with
        | [] -> List.rev itemsToTake, []
        | currentCargo :: _ when not (toTake itemsToTake currentCargo) -> List.rev itemsToTake, items
        | currentCargo :: tail when (toTake itemsToTake currentCargo) = true ->
            splitInternal tail (currentCargo :: itemsToTake)
        | _ -> failwith "unknown situation"

    splitInternal items []

let getNextItemDestination warehouse =
    warehouse.storage
    |> List.head
    |> fun x -> x.cargo.destination

let loadItemsFromWarehouse warehouse atTime maxCount =
    match warehouse.storage with
    | [] -> failwith "Unable load from empty warehouse"
    | _ ->
        let nextItemDestination = getNextItemDestination warehouse

        let toTake =
            canLoadCargo nextItemDestination atTime maxCount

        let itemsToLoad, leftItemsInWarehouse = splitItems toTake warehouse.storage

        let warehouseAfterLoad = { warehouse with storage = leftItemsInWarehouse }
        let readyTime = itemsToLoad
                        |> List.map (fun x -> x.unloadTime)
                        |> List.max
        let cargoes = itemsToLoad |> List.map (fun x -> x.cargo)
        warehouseAfterLoad, readyTime, cargoes
