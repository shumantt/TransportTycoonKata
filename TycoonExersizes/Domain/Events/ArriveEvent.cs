namespace TycoonExersizes.Domain.Events
{
    public class ArriveEvent : VehicleEvent
    {
        public ArriveEvent(int vehicleId, Cargo[] cargos, Point location, int arriveTime) 
            : base(vehicleId, cargos, location)
        {
            ArriveTime = arriveTime;
        }

        public int ArriveTime { get; set; }
    }
}