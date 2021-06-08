using System;
using System.Collections.Generic;
using System.Linq;
using TycoonExersizes2.Domain.Events;

namespace TycoonExersizes2.Domain
{
    public class Vehicle
    {
        private readonly List<Cargo> transportingCargo = new();
        private readonly List<VehicleEvent> events = new();

        public Vehicle(int id, VehicleType type, Point location)
        {
            Id = id;
            Type = type;
            Location = location;
        }

        public int Id { get; }
        
        public VehicleType Type { get; }
        public Point Location { get; private set; }
        
        public int LastDepartTime { get; private set; }
        
        public int LastArriveTime { get; private set; }
        public int LastUnloadTime { get; private set; }
        
        public IEnumerable<VehicleEvent> Events => events;

        public int LoadingTime => Type == VehicleType.Ship ? 1 : 0;

        public int MaxCapacity => Type == VehicleType.Ship ? 4 : 1;

        public void DeliverCargo(Cargo[] cargoes, RouteSegment routeSegment)
        {
            if (cargoes.Length > MaxCapacity)
            {
                throw new ArgumentException($"Can't deliver more than {MaxCapacity} cargoes.");
            }

            if (routeSegment.CoveredBy != Type)
            {
                throw new ArgumentException($"Can't cover segment with {Type} vehicle.");
            }
            
            var startLoadingTime = Math.Max(cargoes.Max(x => x.UnloadAt), LastArriveTime);
            var departTime = startLoadingTime + LoadingTime;
            
            Load(cargoes, startLoadingTime, departTime);

            Depart(departTime, routeSegment.TargetPoint);
            
            Location = routeSegment.TargetPoint;
            Arrive(routeSegment.Length);

            var startUnloadingTime = departTime + routeSegment.Length;
            var finishUnloadingTime = startUnloadingTime + LoadingTime;
            Unload(cargoes, startUnloadingTime, finishUnloadingTime);
        }

        public void ComeBack(RouteSegment routeSegment)
        {
            Depart(LastUnloadTime, routeSegment.StartingPoint);
            Location = routeSegment.StartingPoint;
            Arrive(routeSegment.Length);
        }

        private void Unload(Cargo[] cargos, int startUnloadingTime, int finishUnloadingTime)
        {
            events.Add(new UnloadEvent(Id, transportingCargo.ToArray(), Location, startUnloadingTime));
            foreach (var cargo in cargos)
            {
                cargo.Unload(finishUnloadingTime);
            }
            transportingCargo.Clear();
            LastUnloadTime = finishUnloadingTime;
        }

        private void Load(Cargo[] cargos, int loadStartTime, int finishLoadingTime)
        {
            events.Add(new LoadEvent(Id, transportingCargo.ToArray(), Location, loadStartTime));
            transportingCargo.AddRange(cargos);
            foreach (var cargo in cargos)
            {
                cargo.Load(finishLoadingTime);
            }
        }

        private void Arrive(in int travelTime)
        {
            var arriveTime = LastDepartTime + travelTime;
            LastArriveTime = arriveTime;
            events.Add(new ArriveEvent(Id, transportingCargo.ToArray(), Location, arriveTime));
        }

        private void Depart(in int departTime, Point destination)
        {
            LastDepartTime = departTime;
            events.Add(new DepartEvent(Id, transportingCargo.ToArray(), Location, departTime, destination));
        }
    }
}