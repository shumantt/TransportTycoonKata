using System.Collections.Generic;
using TycoonExersizes2.Domain;
using TycoonExersizes2.Infrastructure;
using Xunit;

namespace TycoonExersizes.Tests.Ex2Tests
{
    public class DeliveryServiceTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void TestCalculateDeliveryTime(Cargo[] input, int expected)
        {
            var (routes, vehicles, stocks) = MapBuilder.BuildMap();
            var deliveryService = new DeliveryService(routes, vehicles, stocks);

            var actual = deliveryService.CalculateDeliveryTime(input);
            
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> TestData =>
            new[]
            {
                new object[]{ InputParser.ParseStockItems("A"), 9 },
                new object[]{ InputParser.ParseStockItems("B"), 5 },
                new object[]{ InputParser.ParseStockItems("AB"), 9 },
                new object[]{ InputParser.ParseStockItems("BB"), 5 },
                new object[]{ InputParser.ParseStockItems("ABB"), 9 },
                new object[]{ InputParser.ParseStockItems("AABABBAB"), 23},
                new object[]{ InputParser.ParseStockItems("ABBBABAAABBB"), 39}
            };
    }
}