using System;
using System.Collections.Generic;
using TycoonExersizes.Domain.Events;

namespace TycoonExersizes.Domain
{
    public class Vehicle
    {
        private readonly List<Cargo> transportingCargo = new();
        private readonly List<VehicleEvent> events = new();
        
        public int Id { get; init; }
        
        public VehicleType Type { get; init; }
        public Point Location { get; set; }
        
        public int LastDepartTime { get; private set; }
        
        public int LastArriveTime { get; private set; }

        public IReadOnlyCollection<VehicleEvent> Events => events;

        public void DeliverCargo(Cargo cargo, RouteSegment routeSegment)
        {
            var departTime = Math.Max(cargo.UnloadAt, LastArriveTime);
            
            transportingCargo.Add(cargo);
            cargo.Load(departTime);
            
            Depart(departTime, routeSegment.TargetPoint);
            
            Location = routeSegment.TargetPoint;

            Arrive(routeSegment.Length);
            cargo.Unload(routeSegment.Length);
            
            transportingCargo.Remove(cargo);
        }

        public void ComeBack(RouteSegment routeSegment)
        {
            Depart(LastArriveTime, routeSegment.StartingPoint);
            Location = routeSegment.StartingPoint;
            Arrive(routeSegment.Length);
        }
        
        private void Arrive(in int travelTime)
        {
            LastArriveTime = LastDepartTime + travelTime;
            events.Add(new ArriveEvent(Id, transportingCargo.ToArray(), Location, LastArriveTime));
        }

        private void Depart(in int departTime, Point destination)
        {
            LastDepartTime = departTime;
            events.Add(new DepartEvent(Id, transportingCargo.ToArray(), Location, departTime, destination));
        }
    }
}