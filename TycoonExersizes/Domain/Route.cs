using System.Linq;

namespace TycoonExersizes.Domain
{
    public class Route
    {
        public Route(RouteSegment[] segments)
        {
            Segments = segments;
        }

        public RouteSegment[] Segments { get; }
        public Point TargetPoint => Segments.Last().TargetPoint;
    }
}