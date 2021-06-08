namespace TycoonExersizes2.Domain.Events
{
    public class DepartEvent : VehicleEvent
    {
        public DepartEvent(int vehicleId, Cargo[] cargoes, Point location, int eventTime, Point destination) 
            : base(vehicleId, cargoes, location, eventTime)
        {
            Destination = destination;
        }

        public Point Destination { get; }
        
        public override string EventId => "DEPART";
    }
}