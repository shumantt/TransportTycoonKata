using System;
using System.Linq;
using TycoonExersizes2.Domain;

namespace TycoonExersizes2.Infrastructure
{
    public static class InputParser
    {
        public static Cargo[] ParseStockItems(string input)
        {
            return input
                .Select((x, i) => new Cargo(i,Enum.Parse<Point>(x.ToString())))
                .ToArray();
        }
    }
}