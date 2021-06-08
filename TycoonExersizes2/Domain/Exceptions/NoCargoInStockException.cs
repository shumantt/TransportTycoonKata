using System;

namespace TycoonExersizes2.Domain.Exceptions
{
    public class NoCargoInStockException : Exception
    {
        public NoCargoInStockException(Stock stock) : base($"No cargo in stock at {stock.Location}")
        {
        }
    }
}