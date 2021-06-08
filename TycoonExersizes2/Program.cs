using TycoonExersizes2.Domain;
using TycoonExersizes2.Infrastructure;

namespace TycoonExersizes2
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args[0];
            var cargos = InputParser.ParseStockItems(input);
            var (routes, vehicles, stocks) = MapBuilder.BuildMap();

            var deliveryService = new DeliveryService(routes, vehicles, stocks);
            deliveryService.CalculateDeliveryTime(cargos);
        }
    }
}