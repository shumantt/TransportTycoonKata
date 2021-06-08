using System;
using TycoonExersizes.Domain;
using TycoonExersizes.Infrastructure;

namespace TycoonExersizes
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args[0];
            var cargos = InputParser.ParseStockItems(input);
            var (routes, vehicles) = MapBuilder.BuildMap();

            var deliveryService = new DeliveryService(routes, vehicles);
            deliveryService.CalculateDeliveryTime(cargos);
        }
    }
}