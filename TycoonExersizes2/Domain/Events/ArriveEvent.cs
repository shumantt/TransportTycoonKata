namespace TycoonExersizes2.Domain.Events
{
    public class ArriveEvent : VehicleEvent
    {
        public ArriveEvent(int vehicleId, Cargo[] cargoes, Point location, int eventTime) 
            : base(vehicleId, cargoes, location, eventTime)
        {
        }

        public override string EventId => "ARRIVE";
    }
}