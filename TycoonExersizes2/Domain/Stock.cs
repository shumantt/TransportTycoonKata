using System;
using System.Collections.Generic;
using System.Linq;
using TycoonExersizes2.Domain.Exceptions;

namespace TycoonExersizes2.Domain
{
    public class Stock
    {
        private readonly Queue<Cargo> storage;

        public Point Location { get; }

        public bool HasAnyItems => storage.Count > 0;

        public Stock(Point location)
        {
            Location = location;
            storage = new Queue<Cargo>();
        }

        public Point GetNextCargoTargetPoint()
        {
            if (!HasAnyItems)
            {
                throw new NoCargoInStockException(this);
            }
            return storage.Peek().TargetPoint;
        }

        public void UnloadCargo(Cargo[] cargoes)
        {
            foreach (var cargo in cargoes)
            {
                storage.Enqueue(cargo);
            }
        }

        public Cargo[] ProvideCargoForLoading(int count, Point targetPoint, int atTime)
        {
            if (!HasAnyItems)
            {
                return Array.Empty<Cargo>();
            }
            
            var cargoes = new List<Cargo>();
            while (cargoes.Count < count && HasAnyItems)
            {
                var cargo = storage.Peek();
                var packIsReady = cargo.UnloadAt > atTime && cargoes.Count > 0 && cargoes.Last().UnloadAt != cargo.UnloadAt;
                if (cargo.TargetPoint != targetPoint || packIsReady)
                {
                    break;
                }

                cargoes.Add(storage.Dequeue());
            }

            return cargoes.ToArray();
        }
    }
}