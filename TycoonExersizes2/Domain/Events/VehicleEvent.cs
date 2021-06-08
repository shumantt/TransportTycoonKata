namespace TycoonExersizes2.Domain.Events
{
    public abstract class VehicleEvent
    {
        protected VehicleEvent(int vehicleId, Cargo[] cargoes, Point location, int eventTime)
        {
            Cargoes = cargoes;
            Location = location;
            VehicleId = vehicleId;
            EventTime = eventTime;
        }
        
        
        public abstract string EventId { get; }
        
        public int VehicleId { get; }
        public Cargo[] Cargoes { get; }
        
        public Point Location { get; }
        
        public int EventTime { get; }
    }
}