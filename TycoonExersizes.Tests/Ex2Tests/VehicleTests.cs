using System;
using System.Linq;
using TycoonExersizes2.Domain;
using TycoonExersizes2.Domain.Events;
using Xunit;

namespace TycoonExersizes.Tests.Ex2Tests
{
    public class VehicleTests
    {
        [Fact]
        public void TestMaxCapacity_ForShipIs4()
        {
            var vehicle = new Vehicle(0, VehicleType.Ship, Point.Factory);

            var actual = vehicle.MaxCapacity;

            Assert.Equal(4, actual);
        }

        [Fact]
        public void TestMaxCapacity_ForTruckIs1()
        {
            var vehicle = new Vehicle(0, VehicleType.Truck, Point.Factory);

            var actual = vehicle.MaxCapacity;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void TestLoadingTime_ForShipIs1()
        {
            var vehicle = new Vehicle(0, VehicleType.Ship, Point.Port);

            var actual = vehicle.LoadingTime;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void TestLoadingTime_ForTruckIs0()
        {
            var vehicle = new Vehicle(0, VehicleType.Truck, Point.Factory);

            var actual = vehicle.LoadingTime;

            Assert.Equal(0, actual);
        }

        [Fact]
        public void TestDeliverCargo_ThrowsWhenPasseMoreCargoesThanMaxCapacity()
        {
            var vehicle = new Vehicle(0, VehicleType.Truck, Point.Factory);

            Action deliver = () => vehicle.DeliverCargo(new Cargo[2], new RouteSegment());

            Assert.Throws<ArgumentException>(deliver);
        }
        
        [Fact]
        public void TestDeliverCargo_ThrowsWhenPassUncoveredSegment()
        {
            var vehicle = new Vehicle(0, VehicleType.Truck, Point.Factory);

            Action deliver = () => vehicle.DeliverCargo(new Cargo[1], new RouteSegment { CoveredBy = VehicleType.Ship });

            Assert.Throws<ArgumentException>(deliver);
        }

        [Fact]
        public void TestDeliverCargo_VehicleGetsToTargetSegmentLocation()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery();

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(Point.B, vehicle.Location);
        }

        [Fact]
        public void TestDeliverCargo_DepartTimeIsZeroWithoutLoadingForTheFirstTrip()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery();

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(0, vehicle.LastDepartTime);
        }
        
        [Fact]
        public void TestDeliverCargo_RecordsArriveTime()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery();

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(5, vehicle.LastArriveTime);
        }
        
        [Fact]
        public void TestDeliverCargo_RecordsUnloadTimeSameAsArriveTimeWithoutUnloading()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery();

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(5, vehicle.LastUnloadTime);
        }
        
        [Fact]
        public void TestDeliverCargo_SetsUnloadCargoTime()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery();

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Collection(cargoes, c => Assert.Equal(5, c.UnloadAt));
        }
        
        [Fact]
        public void TestDeliverCargo_RecordDepartTimeWithLoading()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(VehicleType.Ship);

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(1, vehicle.LastDepartTime);
        }

        [Fact]
        public void TestDeliverCargo_RecordDepartTimeFromLastArriveTime_IfCargoIsReady()
        {
            var (vehicle, cargoes0, segment0) = CreateBasicDelivery();
            vehicle.DeliverCargo(cargoes0, segment0);
            
            var (_, cargoes, segment) = CreateBasicDelivery();
            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(5, vehicle.LastDepartTime);
        }
        
        [Fact]
        public void TestDeliverCargo_RecordDepartTimeFromLastCargoUnload_IfItIsLaterLastTimeArrive()
        {
            var (vehicle, _, segment) = CreateBasicDelivery();
            var cargo = new Cargo(0, Point.B);
            cargo.Unload(1);
            vehicle.DeliverCargo(new [] { cargo }, segment);
            
            Assert.Equal(1, vehicle.LastDepartTime);
        }
        
        [Fact]
        public void TestDeliverCargo_RecordDepartTimeFromMaxLastCargoUnload_IfItIsLaterLastTimeArrive()
        {
            var (vehicle, _, segment) = CreateBasicDelivery(VehicleType.Ship);
            var cargo1 = new Cargo(0, Point.B);
            cargo1.Unload(1);
            var cargo2 = new Cargo(0, Point.B);
            cargo2.Unload(2);
            
            vehicle.DeliverCargo(new [] {cargo1, cargo2 }, segment);
            
            Assert.Equal(3, vehicle.LastDepartTime);
        }

        [Fact]
        public void TestDeliverCargo_RecordsUnloadTimeWithUnloading()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(VehicleType.Ship);

            vehicle.DeliverCargo(cargoes, segment);
            
            Assert.Equal(7, vehicle.LastUnloadTime);
        }

        [Fact]
        public void TestDeliverCargo_GeneratesLoadEvent()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(id: 100);

            vehicle.DeliverCargo(cargoes, segment);

            var firstEvent = vehicle.Events.ElementAt(0);
            Assert.IsType<LoadEvent>(firstEvent);

            var loadEvent = (LoadEvent) firstEvent;
            
            Assert.Empty(loadEvent.Cargoes);
            Assert.Equal(Point.Factory, loadEvent.Location);
            Assert.Equal(100, loadEvent.VehicleId);
            Assert.Equal(0, loadEvent.EventTime);
        }
        
        [Fact]
        public void TestDeliverCargo_GeneratesDepartEvent()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(id: 100);

            vehicle.DeliverCargo(cargoes, segment);

            var vehicleEvent = vehicle.Events.ElementAt(1);
            Assert.IsType<DepartEvent>(vehicleEvent);

            var departEvent = (DepartEvent) vehicleEvent;
            
            Assert.NotEmpty(departEvent.Cargoes);
            Assert.Equal(Point.Factory, departEvent.Location);
            Assert.Equal(Point.B, departEvent.Destination);
            Assert.Equal(100, departEvent.VehicleId);
            Assert.Equal(0, departEvent.EventTime);
        }
        
        [Fact]
        public void TestDeliverCargo_GeneratesArriveEvent()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(id: 100);

            vehicle.DeliverCargo(cargoes, segment);

            var vehicleEvent = vehicle.Events.ElementAt(2);
            Assert.IsType<ArriveEvent>(vehicleEvent);

            var arriveEvent = (ArriveEvent) vehicleEvent;
            
            Assert.NotEmpty(arriveEvent.Cargoes);
            Assert.Equal(Point.B, arriveEvent.Location);
            Assert.Equal(100, arriveEvent.VehicleId);
            Assert.Equal(5, arriveEvent.EventTime);
        }
        
        [Fact]
        public void TestDeliverCargo_GeneratesUnloadEvent()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(id: 100);

            vehicle.DeliverCargo(cargoes, segment);

            var vehicleEvent = vehicle.Events.ElementAt(3);
            Assert.IsType<UnloadEvent>(vehicleEvent);

            var unloadEvent = (UnloadEvent) vehicleEvent;
            
            Assert.NotEmpty(unloadEvent.Cargoes);
            Assert.Equal(Point.B, unloadEvent.Location);
            Assert.Equal(100, unloadEvent.VehicleId);
            Assert.Equal(5, unloadEvent.EventTime);
        }

        [Fact]
        public void TestComeBack_SetLocationToStartingPoint()
        {
            var (vehicle, _, segment) = CreateBasicDelivery();
            
            vehicle.ComeBack(segment);
            
            Assert.Equal(Point.Factory, vehicle.Location);
        }
        
        [Fact]
        public void TestComeBack_DepartsAfterUnload()
        {
            var (vehicle, cargoes, segment) = CreateBasicDelivery(VehicleType.Ship);

            vehicle.DeliverCargo(cargoes, segment);
            
            vehicle.ComeBack(segment);
            
            Assert.Equal(7, vehicle.LastDepartTime);
        }
        
        [Fact]
        public void TestComeBack_ArrivesInSegmentTravelTime()
        {
            var (vehicle, _, segment) = CreateBasicDelivery();
            
            vehicle.ComeBack(segment);
            
            Assert.Equal(5, vehicle.LastArriveTime);
        }
        
        [Fact]
        public void TestComeBack_GeneratesDepartEvent()
        {
            var (vehicle, _, segment) = CreateBasicDelivery(id: 100, vehicleLocation: Point.B);

            vehicle.ComeBack(segment);

            var vehicleEvent = vehicle.Events.ElementAt(0);
            Assert.IsType<DepartEvent>(vehicleEvent);

            var departEvent = (DepartEvent) vehicleEvent;
            
            Assert.Empty(departEvent.Cargoes);
            Assert.Equal(Point.B, departEvent.Location);
            Assert.Equal(Point.Factory, departEvent.Destination);
            Assert.Equal(100, departEvent.VehicleId);
            Assert.Equal(0, departEvent.EventTime);
        }
        
        [Fact]
        public void TestComeBack_GeneratesArriveEvent()
        {
            var (vehicle, _, segment) = CreateBasicDelivery(id: 100, vehicleLocation: Point.B);

            vehicle.ComeBack(segment);

            var vehicleEvent = vehicle.Events.ElementAt(1);
            Assert.IsType<ArriveEvent>(vehicleEvent);

            var arriveEvent = (ArriveEvent) vehicleEvent;
            
            Assert.Empty(arriveEvent.Cargoes);
            Assert.Equal(Point.Factory, arriveEvent.Location);
            Assert.Equal(100, arriveEvent.VehicleId);
            Assert.Equal(5, arriveEvent.EventTime);
        }

        private static (Vehicle vehicle, Cargo[] cargoes, RouteSegment segment) 
            CreateBasicDelivery(
                VehicleType vehicleType = VehicleType.Truck,
                int id = 0,
                VehicleType? coveredBy = null,
                Point vehicleLocation = Point.Factory
        )
        {
            var vehicle = new Vehicle(id, vehicleType, vehicleLocation);

            var segmentCoverage = coveredBy ?? vehicleType;

            var cargoes = new[] {new Cargo(0, Point.B)};
            var segment = new RouteSegment {StartingPoint = Point.Factory, TargetPoint = Point.B, Length = 5, CoveredBy = segmentCoverage};
            return (vehicle, cargoes, segment);
        }
    }
}