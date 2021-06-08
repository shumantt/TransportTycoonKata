namespace TycoonExersizes.Domain
{
    public class RouteSegment
    {
        public Point StartingPoint { get; init; }
        public Point TargetPoint { get; init; }
        public VehicleType CoveredBy { get; init; }
        public int Length { get; init; }
    }
}