namespace TycoonExersizes.Domain.Events
{
    public abstract class VehicleEvent
    {
        protected VehicleEvent(int vehicleId, Cargo[] cargos, Point location)
        {
            Cargos = cargos;
            Location = location;
            VehicleId = vehicleId;
        }
        
        public int VehicleId { get; set; }
        public Cargo[] Cargos { get; set; }
        
        public Point Location { get; set; }
    }
}