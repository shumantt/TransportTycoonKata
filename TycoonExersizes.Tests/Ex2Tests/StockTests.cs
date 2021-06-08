using System;
using System.Collections.Generic;
using TycoonExersizes2.Domain;
using TycoonExersizes2.Domain.Exceptions;
using Xunit;

namespace TycoonExersizes.Tests.Ex2Tests
{
    public class StockTests
    {
        [Fact]
        public void TestHasAnyItems_ReturnsFalseWithoutUnloadedCargo()
        {
            var stock = new Stock(Point.Factory);

            var actual = stock.HasAnyItems;

            Assert.False(actual);
        }
        
        [Fact]
        public void TestHasAnyItems_ReturnsTrueAfterCargoUnload()
        {
            var stock = new Stock(Point.Factory);

            stock.UnloadCargo(new []{ new Cargo(0, Point.A) });
            var actual = stock.HasAnyItems;

            Assert.True(actual);
        }
        
        [Fact]
        public void TestGetNextCargoTargetPoint_ThrowIfNoItems()
        {
            var stock = new Stock(Point.Factory);

            Action getNextTargetPoint = () => stock.GetNextCargoTargetPoint();

            Assert.Throws<NoCargoInStockException>(getNextTargetPoint);
        }

        [Theory]
        [MemberData(nameof(GetNextCargoTargetPointTestData))]
        public void TestGetNextCargoTargetPoint_ReturnTargetPointOfTheFirstItem(Cargo[] cargoes, Point expected)
        {
            var stock = new Stock(Point.Factory);
            stock.UnloadCargo(cargoes);
            
            var actual = stock.GetNextCargoTargetPoint();
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestProvideCargoForLoading_ReturnsNothingOnEmptyStock()
        {
            var stock = new Stock(Point.Factory);

            var actual = stock.ProvideCargoForLoading(0, Point.A, 0);
            
            Assert.Empty(actual);
        }
        
        [Fact]
        public void TestProvideCargoForLoading_ReturnsAllApplicableCargoByTargetPoint()
        {
            var stock = new Stock(Point.Factory);

            stock.UnloadCargo(new Cargo[]
            {
                new (0, Point.A),
                new (1, Point.A)
            });
            
            var actual = stock.ProvideCargoForLoading(3, Point.A, 0);
            
            Assert.Equal(2, actual.Length);
            Assert.All(actual, c => Assert.Equal(Point.A, c.TargetPoint));
        }
        
        [Fact]
        public void TestProvideCargoForLoading_ReturnsCargoWithTargetPointInRow()
        {
            var stock = new Stock(Point.Factory);

            stock.UnloadCargo(new Cargo[]
            {
                new (0, Point.A),
                new (1, Point.B),
                new (2, Point.A)
            });
            
            var actual = stock.ProvideCargoForLoading(3, Point.A, 0);
            
            Assert.Single(actual);
            Assert.Equal(Point.A, actual[0].TargetPoint);
        }
        
        [Fact]
        public void TestProvideCargoForLoading_LimitsProvidedCargo()
        {
            var stock = new Stock(Point.Factory);

            stock.UnloadCargo(new Cargo[]
            {
                new (0, Point.A),
                new (1, Point.A)
            });
            
            var actual = stock.ProvideCargoForLoading(1, Point.A, 0);
            
            Assert.Single(actual);
            Assert.Equal(Point.A, actual[0].TargetPoint);
        }
        
        [Fact]
        public void TestProvideCargoForLoading_ReturnOnlyAtUnloadTime()
        {
            var stock = new Stock(Point.Factory);
            
            var cargo1 = new Cargo(0, Point.A);
            cargo1.Unload(1);
            var cargo2 = new Cargo(1, Point.A);
            cargo2.Unload(3);
            
            stock.UnloadCargo(new []
            {
                cargo1, cargo2
            });
            
            var actual = stock.ProvideCargoForLoading(3, Point.A, 2);
            
            Assert.Single(actual);
            Assert.Equal(Point.A, actual[0].TargetPoint);
            Assert.True(actual[0].UnloadAt <= 2);
        }
        
        [Fact]
        public void TestProvideCargoForLoading_ReturnAtLeastOneCargoAtLaterTime()
        {
            var stock = new Stock(Point.Factory);
            
            var cargo1 = new Cargo(0, Point.A);
            cargo1.Unload(2);
            var cargo2 = new Cargo(1, Point.A);
            cargo2.Unload(3);
            
            stock.UnloadCargo(new []
            {
                cargo1, cargo2
            });
            
            var actual = stock.ProvideCargoForLoading(3, Point.A, 1);
            
            Assert.Single(actual);
            Assert.Equal(Point.A, actual[0].TargetPoint);
        }
        
        [Fact]
        public void TestProvideCargoForLoading_ReturnAllCargoAtLaterTimeUnloadedAtTheSameTime()
        {
            var stock = new Stock(Point.Factory);
            
            var cargo1 = new Cargo(0, Point.A);
            cargo1.Unload(2);
            var cargo2 = new Cargo(1, Point.A);
            cargo2.Unload(2);
            
            stock.UnloadCargo(new []
            {
                cargo1, cargo2
            });
            
            var actual = stock.ProvideCargoForLoading(3, Point.A, 1);
            
            Assert.Equal(2, actual.Length);
            Assert.All(actual, c => Assert.Equal(Point.A, c.TargetPoint));
        }
        
        [Fact]
        public void TestProvideCargoForLoading_ActuallyRemovesItemsFromStock()
        {
            var stock = new Stock(Point.Factory);

            stock.UnloadCargo(new Cargo[]
            {
                new (0, Point.A),
                new (1, Point.A)
            });
            
            var _ = stock.ProvideCargoForLoading(2, Point.A, 0);
            
            Assert.False(stock.HasAnyItems);
        }

        public static IEnumerable<object[]> GetNextCargoTargetPointTestData =>
            new List<object[]>
            {
                new object[]
                {
                    new []
                    {
                        new Cargo(0, Point.A)
                    },
                    Point.A
                },
                
                new object[]
                {
                    new Cargo[]
                    {
                        new(1, Point.B),
                        new(2, Point.A)
                    },
                    Point.B
                },
            };
    }
}