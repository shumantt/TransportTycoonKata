using System;
using System.Linq;
using TycoonExersizes.Domain.Events;
using TycoonExersizes.Infrastructure;

namespace TycoonExersizes.Domain
{
    public class DeliveryService
    {
        private readonly Route[] routes;
        private readonly Vehicle[] vehicles;

        public DeliveryService(Route[] routes, Vehicle[]  vehicles)
        {
            this.routes = routes;
            this.vehicles = vehicles;
        }
        
        public int CalculateDeliveryTime(Cargo[] cargoes)
        {
            foreach (var cargo in cargoes)
            {
                var cargoRoute = routes.First(x => x.TargetPoint == cargo.TargetPoint);
                foreach (var segment in cargoRoute.Segments)
                {
                    var vehicle = vehicles
                        .Where(x => x.Location == segment.StartingPoint && x.Type == segment.CoveredBy)
                        .OrderBy(x => x.LastArriveTime)
                        .First();
                    vehicle.DeliverCargo(cargo,segment);
                    vehicle.ComeBack(segment);
                }
            }

            foreach (var vehicleEvent in vehicles.SelectMany(x => x.Events).Select(ConvertToLogEvent))
            {
                EventsLogger.Log(vehicleEvent);
            }
            
            return cargoes.Max(x => x.UnloadAt);
        }


        private LogEvent ConvertToLogEvent(VehicleEvent vehicleEvent)
        {
            return vehicleEvent switch
            {
                ArriveEvent arriveEvent => ConvertArriveEvent(arriveEvent),
                DepartEvent departEvent => ConvertDepartEvent(departEvent),
                _ => throw new ArgumentOutOfRangeException(nameof(vehicleEvent))
            };
        }

        private LogEvent ConvertDepartEvent(DepartEvent departEvent)
        {
            var vehicle = vehicles.First(x => x.Id == departEvent.VehicleId);
            return new LogEvent
            {
                @event = "DEPART",
                time = departEvent.DepartTime,
                transport_id = departEvent.VehicleId,
                kind = vehicle.Type.ToString().ToUpper(),
                location = departEvent.Location.ToString().ToUpper(),
                destination = departEvent.Destination.ToString().ToUpper(),
                cargo = departEvent.Cargos.Select(ConvertCargo).ToArray()
            };

        }

        private LogEvent ConvertArriveEvent(ArriveEvent arriveEvent)
        {
            var vehicle = vehicles.First(x => x.Id == arriveEvent.VehicleId);
            return new LogEvent
            {
                @event = "ARRIVE",
                time = arriveEvent.ArriveTime,
                transport_id = arriveEvent.VehicleId,
                kind = vehicle.Type.ToString().ToUpper(),
                location = arriveEvent.Location.ToString().ToUpper(),
                cargo = arriveEvent.Cargos.Select(ConvertCargo).ToArray()
            };
        }

        private CargoLog ConvertCargo(Cargo cargo)
        {
            return new()
            {
                cargo_id = cargo.Id,
                destination = cargo.TargetPoint.ToString().ToUpper()
            };
        }
    }
}