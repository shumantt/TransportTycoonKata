// ReSharper disable InconsistentNaming
#pragma warning disable 8618
namespace TycoonExersizes2.Infrastructure
{
    public class LogEvent
    {
        public string @event { get; set; }
        public int time { get; set; }
        public int transport_id { get; set; }
        public string kind { get; set; }
        public string location { get; set; }
        public string destination { get; set; }
        public CargoLog[] cargo { get; set; }
        
        public int duration { get; set; }
    }

    public class CargoLog
    {
        public int cargo_id { get; set; }
        public string destination {get; set; }
        public string origin { get; } = "FACTORY";
    }
}