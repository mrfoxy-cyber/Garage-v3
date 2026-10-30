using Garage.Vehicles;
using Vehicles.Interfaces;

namespace Garage_V3
{


    public sealed class Garage<T> : IEnumerable<T>
    where T : class, IVehicle
    {
        public const int MaximumGarageCapacity = 100_000;

        public string GarageName { get; }
        public int Capacity { get; }

        public List<(int SpotNumber, T Vehicle)> Vehicles { get; } = [];

        public int OccupiedSpaces => Vehicles.Count;
        public int AvailableSpaces => Capacity - OccupiedSpaces;

        public Garage(string garageName, int capacity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(garageName);

            if (capacity < 1 || capacity > MaximumGarageCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            GarageName = garageName.Trim();
            Capacity = capacity;
        }

        public IEnumerator<T> GetEnumerator() => Vehicles.Select(entry => entry.Vehicle).GetEnumerator();

        System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator() =>
                GetEnumerator();
    }

}
