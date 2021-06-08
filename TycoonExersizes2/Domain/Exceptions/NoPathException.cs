using System;

namespace TycoonExersizes2.Domain.Exceptions
{
    public class NoPathException : Exception
    {
        public NoPathException(Point startingPoint) : base($"No path for {startingPoint} as starting point")
        {
        }
    }
}