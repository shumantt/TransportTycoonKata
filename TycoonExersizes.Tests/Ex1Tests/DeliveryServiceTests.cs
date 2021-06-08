using System.Collections.Generic;
using TycoonExersizes.Domain;
using TycoonExersizes.Infrastructure;
using Xunit;

namespace TycoonExersizes.Tests.Ex1Tests
{
    public class DeliveryServiceTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void TestCalculateDeliveryTime(Cargo[] input, int expected)
        {
            var (routes, vehicles) = MapBuilder.BuildMap();
            var deliveryService = new DeliveryService(routes, vehicles);

            var actual = deliveryService.CalculateDeliveryTime(input);
            
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> TestData =>
            new[]
            {
                new object[]{ InputParser.ParseStockItems("A"), 5 },
                new object[]{ InputParser.ParseStockItems("B"), 5 },
                new object[]{ InputParser.ParseStockItems("AB"), 5 },
                new object[]{ InputParser.ParseStockItems("BB"), 5 },
                new object[]{ InputParser.ParseStockItems("ABB"), 7 },
                new object[]{ InputParser.ParseStockItems("AABABBAB"), 29},
                new object[]{ InputParser.ParseStockItems("ABBBABAAABBB"), 41}
            };
    }
}