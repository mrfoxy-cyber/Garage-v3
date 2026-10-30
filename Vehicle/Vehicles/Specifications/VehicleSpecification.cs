using Garage.Engines;
using Garage.Vehicles.Components;

namespace Garage.Vehicles.Specifications;

public abstract class VehicleSpecification
{
    public const int MaximumNumberOfEngines = 100;
    public const int MaximumNumberOfSeats = 10_000;
    public const int MaximumNumberOfWheels = 100;
    public const double MaximumLengthMeters = 1_000;

    public double? WeightKg { get; init; }
    public required Dimensions Dimensions { get; init; }
    private List<Engine> engines = [];

    public List<Engine> Engines
    {
        get => new List<Engine>(engines);
        init
        {
            if (value.Count > MaximumNumberOfEngines)
            {
                throw new ArgumentException(
                    $"A vehicle cannot have more than {MaximumNumberOfEngines} engines.",
                    nameof(value));
            }

            engines = new List<Engine>(value);
        }
    }

    public void AddEngine (Engine engine)
    {
        if (engines.Count >= MaximumNumberOfEngines)
        {
            throw new InvalidOperationException(
                $"A vehicle cannot have more than " +
                $"{MaximumNumberOfEngines} engines.");
        }

        engines.Add(engine);
    }

    public Engine? FindEngine(string serialNumber)
    {
        return engines.Find(engine => string.Equals(
                                                    engine.SerialNumber,
                                                    serialNumber,
                                                    StringComparison.OrdinalIgnoreCase));
    }

    public int FindEngineIndex(string serialNumber)
    {
        return engines.FindIndex(engine => string.Equals(
                                                    engine.SerialNumber,
                                                    serialNumber,
                                                    StringComparison.OrdinalIgnoreCase));
    }

    public bool RemoveEngine (Engine engine)
    {
        int index = FindEngineIndex(engine.SerialNumber);

        if(index ==-1)
        {
            return false;
        }

        engines.RemoveAt(index);

        return true;
    }
}
