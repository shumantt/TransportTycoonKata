namespace TycoonExersizes2.Domain
{
    public class Cargo
    {
        public Cargo(int id, Point targetPoint)
        {
            Id = id;
            TargetPoint = targetPoint;
        }

        public int Id { get; }
        public Point TargetPoint { get; }
        public int LoadAt { get; private set; }
        public int UnloadAt { get; private set; }

        public void Load(int loadAt)
        {
            LoadAt = loadAt;
        }

        public void Unload(int unloadAt)
        {
            UnloadAt = unloadAt;
        }
    }
}