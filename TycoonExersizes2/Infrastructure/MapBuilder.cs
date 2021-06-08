using TycoonExersizes2.Domain;

namespace TycoonExersizes2.Infrastructure
{
    public static class MapBuilder
    {
        public static (Route[] routes, Vehicle[] vehicles, Stock[] stocks) BuildMap()
        {
            var routes = BuildRoutes();
            var vehicles = BuildVehicles();
            var stocks = BuildStocks();

            return (routes, vehicles, stocks);
        }

        private static Stock[] BuildStocks()
        {
            return new[]
            {
                new Stock(Point.Factory),
                new Stock(Point.Port)
            };
        }

        private static Vehicle[] BuildVehicles()
        {
            return new[]
            {
                BuildTruck(1),
                BuildTruck(2),
                new (3, VehicleType.Ship, Point.Port)
            };

            Vehicle BuildTruck(int id)
            {
                return new(id, VehicleType.Truck, Point.Factory);
            }
        }

        private static Route[] BuildRoutes()
        {
            var toA = new Route(new[]
            {
                new RouteSegment
                {
                    StartingPoint = Point.Factory,
                    TargetPoint = Point.Port,
                    CoveredBy = VehicleType.Truck,
                    Length = 1
                },
                new RouteSegment
                {
                    StartingPoint = Point.Port,
                    TargetPoint = Point.A,
                    CoveredBy = VehicleType.Ship,
                    Length = 6
                }
            });

            var toB = new Route(new[]
            {
                new RouteSegment
                {
                    StartingPoint = Point.Factory,
                    TargetPoint = Point.B,
                    CoveredBy = VehicleType.Truck,
                    Length = 5
                }
            });

            return new[] {toA, toB};
        }
    }
}