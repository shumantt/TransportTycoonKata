using System;
using System.Linq;
using TycoonExersizes.Domain;

namespace TycoonExersizes.Infrastructure
{
    public class InputParser
    {
        public static Cargo[] ParseStockItems(string input)
        {
            return input
                .Select((x, i) => new Cargo
                {
                    Id = i,
                    TargetPoint = Enum.Parse<Point>(x.ToString())
                })
                .ToArray();
        }
    }
}