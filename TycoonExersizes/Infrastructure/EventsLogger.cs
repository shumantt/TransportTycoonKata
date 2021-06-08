using System;
using System.Text.Json;

namespace TycoonExersizes.Infrastructure
{
    public class EventsLogger
    {
        public static void Log(LogEvent logEvent)
        {
            Console.WriteLine(JsonSerializer.Serialize(logEvent));
        }
    }
}