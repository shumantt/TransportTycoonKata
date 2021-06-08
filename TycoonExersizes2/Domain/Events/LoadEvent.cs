namespace TycoonExersizes2.Domain.Events
{
    public class LoadEvent : VehicleEvent
    {
        public LoadEvent(int vehicleId, Cargo[] cargoes, Point location, int eventTime) 
            : base(vehicleId, cargoes, location, eventTime)
        {
        }

        public override string EventId => "LOAD";
    }
}