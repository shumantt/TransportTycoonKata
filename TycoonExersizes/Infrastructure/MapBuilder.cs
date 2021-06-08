using TycoonExersizes.Domain;

namespace TycoonExersizes.Infrastructure
{
    public static class MapBuilder
    {
        public static (Route[] routes, Vehicle[] vehicles) BuildMap()
        {
            var routes = BuildRoutes();
            var vehicles = BuildVehicles();

            return (routes, vehicles);
        }

        private static Vehicle[] BuildVehicles()
        {
            return new[]
            {
                BuildTruck(1),
                BuildTruck(2),
                new Vehicle
                {
                    Id = 3,
                    Location = Point.Port,
                    Type = VehicleType.Ship
                }
            };

            Vehicle BuildTruck(int id)
            {
                return new()
                {
                    Id = id,
                    Location = Point.Factory,
                    Type = VehicleType.Truck
                };
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
                    Length = 4
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