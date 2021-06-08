namespace TycoonExersizes2.Domain.Events
{
    public class UnloadEvent : VehicleEvent
    {
        public UnloadEvent(int vehicleId, Cargo[] cargoes, Point location, int eventTime) 
            : base(vehicleId, cargoes, location, eventTime)
        {
        }

        public override string EventId => "UNLOAD";
    }
}