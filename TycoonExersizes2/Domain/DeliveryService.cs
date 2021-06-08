using System.Linq;
using TycoonExersizes2.Domain.Events;
using TycoonExersizes2.Infrastructure;

namespace TycoonExersizes2.Domain
{
    public class DeliveryService
    {
        private readonly Route[] routes;
        private readonly Vehicle[] vehicles;
        private readonly Stock[] stocks;

        public DeliveryService(Route[] routes, Vehicle[] vehicles, Stock[] stocks)
        {
            this.routes = routes;
            this.vehicles = vehicles;
            this.stocks = stocks;
        }

        public int CalculateDeliveryTime(Cargo[] cargoes)
        {
            stocks.First(x => x.Location == Point.Factory).UnloadCargo(cargoes);

            foreach (var stock in stocks)
            {
                while (stock.HasAnyItems)
                {
                    var nextCargoTargetPoint = stock.GetNextCargoTargetPoint();
                    var route = routes.First(x => x.TargetPoint == nextCargoTargetPoint);
                    var segment = route.GetNextSegment(stock.Location);
                    var vehicle = FindVehicle(segment);
                    var cargoesLoad = stock.ProvideCargoForLoading(vehicle.MaxCapacity, nextCargoTargetPoint, vehicle.LastArriveTime);

                    vehicle.DeliverCargo(cargoesLoad, segment);

                    var targetStock = stocks.FirstOrDefault(x => x.Location == segment.TargetPoint);
                    targetStock?.UnloadCargo(cargoesLoad);

                    vehicle.ComeBack(segment);
                }
            }

            foreach (var vehicleEvent in vehicles.SelectMany(x => x.Events).Select(ConvertToLogEvent))
            {
                EventsLogger.Log(vehicleEvent);
            }

            return cargoes.Max(x => x.UnloadAt);
        }

        private Vehicle FindVehicle(RouteSegment segment) =>
            vehicles
                .Where(x => x.Location == segment.StartingPoint && x.Type == segment.CoveredBy)
                .OrderBy(x => x.LastArriveTime)
                .First();

        private LogEvent ConvertToLogEvent(VehicleEvent vehicleEvent)
        {
            return vehicleEvent switch
            {
                DepartEvent departEvent => ConvertDepartEvent(departEvent),
                _ => Convert(vehicleEvent)
            };
        }

        private LogEvent Convert(VehicleEvent vehicleEvent)
        {
            var vehicle = vehicles.First(x => x.Id == vehicleEvent.VehicleId);
            return new LogEvent
            {
                @event = vehicleEvent.EventId,
                time = vehicleEvent.EventTime,
                duration = vehicle.LoadingTime,
                transport_id = vehicleEvent.VehicleId,
                kind = vehicle.Type.ToString().ToUpper(),
                location = vehicleEvent.Location.ToString().ToUpper(),
                cargo = vehicleEvent.Cargoes.Select(ConvertCargo).ToArray()
            };
        }

        private LogEvent ConvertDepartEvent(DepartEvent departEvent)
        {
            var logEvent = Convert(departEvent);
            logEvent.destination = departEvent.Destination.ToString().ToUpper();
            return logEvent;
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