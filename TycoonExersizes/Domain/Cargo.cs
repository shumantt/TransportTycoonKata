using System;

namespace TycoonExersizes.Domain
{
    public class Cargo
    {
        public int Id { get; init; }
        public Point TargetPoint { get; init; }
        public int LoadAt { get; private set; }
        public int UnloadAt { get; private set; }

        public void Load(int loadAt)
        {
            LoadAt = loadAt;
        }

        public void Unload(int travelTime)
        {
            UnloadAt = LoadAt + travelTime;
        }
    }
}