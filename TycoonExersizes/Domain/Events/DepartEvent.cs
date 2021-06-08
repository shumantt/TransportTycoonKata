namespace TycoonExersizes.Domain.Events
{
    public class DepartEvent : VehicleEvent
    {
        public DepartEvent(int vehicleId, Cargo[] cargos, Point location, int departTime, Point destination) 
            : base(vehicleId, cargos, location)
        {
            DepartTime = departTime;
            Destination = destination;
        }

        public int DepartTime { get; }
        public Point Destination { get; }
    }
}