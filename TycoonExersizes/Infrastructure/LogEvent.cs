// ReSharper disable InconsistentNaming
#pragma warning disable 8618
namespace TycoonExersizes.Infrastructure
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
    }

    public class CargoLog
    {
        public int cargo_id { get; set; }
        public string destination {get; set; }
        public string origin { get; } = "FACTORY";
    }
    
    /*
     * {
  "event": "DEPART",     # type of log entry: DEPART or ARRIVE
  "time": 0,             # time in hours
  "transport_id": 0,     # unique transport id
  "kind": "TRUCK",       # transport kind
  "location": "FACTORY", # current location
  "destination": "PORT", # destination (only for DEPART events)
  "cargo": [             # array of cargo being carried
    {
      "cargo_id": 0,     # unique cargo id
      "destination": "A",# where should the cargo be delivered
      "origin": "FACTORY"# where it is originally from
    }
  ]
}
     */
}