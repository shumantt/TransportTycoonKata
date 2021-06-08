using System;
using System.Linq;
using TycoonExersizes2.Domain.Exceptions;

namespace TycoonExersizes2.Domain
{
    public class Route
    {
        private readonly RouteSegment[] segments;
        
        public Route(RouteSegment[] segments)
        {
            ValidateSegments(segments);
            
            this.segments = segments;
        }
        
        public Point TargetPoint => segments.Last().TargetPoint;

        public RouteSegment GetNextSegment(Point startingPoint) =>
            segments.FirstOrDefault(x => x.StartingPoint == startingPoint) ?? throw new NoPathException(startingPoint);

        private static void ValidateSegments(RouteSegment[] segments)
        {
            if (segments.Length == 0)
            {
                throw new ArgumentException("Route must contain at least one segment");
            }

            for (int i = 1; i < segments.Length; i++)
            {
                if (segments[i - 1].TargetPoint != segments[i].StartingPoint)
                {
                    throw new ArgumentException("Segments should go from one point to the next in order");
                }
            }
        }
    }
}