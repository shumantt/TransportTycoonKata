using System;
using TycoonExersizes2.Domain;
using TycoonExersizes2.Domain.Exceptions;
using Xunit;

namespace TycoonExersizes.Tests.Ex2Tests
{
    public class RouteTests
    {
        [Fact]
        public void TestCantCreateWithoutSegments()
        {
            Func<Route> create = () => new Route(new RouteSegment[0]);
            Assert.Throws<ArgumentException>(create);
        }
        
        [Fact]
        public void TestCantCreateInInvalidOrder()
        {
            Func<Route> create = () => new Route(new []
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
                    StartingPoint = Point.A,
                    TargetPoint = Point.B,
                    CoveredBy = VehicleType.Truck,
                    Length = 1
                }
            });
            Assert.Throws<ArgumentException>(create);
        }

        [Fact]
        public void TestTargetPointWithSingleSegment_ShouldReturnItsTargetPoint()
        {
            var route = new Route(new[]
            {
                new RouteSegment
                {
                    StartingPoint = Point.Factory,
                    TargetPoint = Point.Port,
                    CoveredBy = VehicleType.Truck,
                    Length = 1
                }
            });

            var actual = route.TargetPoint;
            
            Assert.Equal(Point.Port, actual);
        }
        
        [Fact]
        public void TestTargetPointWithSeveralSegment_ShouldReturnTargetPointOfLastSegment()
        {
            var route = new Route(new[]
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
                    Length = 1
                }
            });

            var actual = route.TargetPoint;
            
            Assert.Equal(Point.A, actual);
        }

        [Fact]
        public void TestGetNextSegment_ThrowsOnNotExistingPoint()
        {
            var route = new Route(new[]
            {
                new RouteSegment
                {
                    StartingPoint = Point.Factory,
                    TargetPoint = Point.Port,
                    CoveredBy = VehicleType.Truck,
                    Length = 1
                }
            });

            Func<RouteSegment> getSegment = () => route.GetNextSegment(Point.Port);
            
            Assert.Throws<NoPathException>(getSegment);
        }
        
        [Fact]
        public void TestGetNextSegment_ReturnSegmentWithStartingPoint()
        {
            var route = new Route(new[]
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
                    Length = 1
                }
            });

            var actual = route.GetNextSegment(Point.Port);
            
            Assert.Equal(Point.Port, actual.StartingPoint);
            Assert.Equal(Point.A, actual.TargetPoint);
        }
    }
}