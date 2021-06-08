using System;
using System.Text.Json;

namespace TycoonExersizes2.Infrastructure
{
    public static class EventsLogger
    {
        public static void Log(LogEvent logEvent)
        {
            Console.WriteLine(JsonSerializer.Serialize(logEvent));
        }
    }
}