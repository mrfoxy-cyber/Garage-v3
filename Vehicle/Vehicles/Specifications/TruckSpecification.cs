namespace Garage.Vehicles.Specifications;

public class TruckSpecification : VehicleSpecification
{
    public TruckSpecification(
        int numberOfWheels,
        int numberOfAxles,
        double maximumLoadKg)
    {
        if (numberOfAxles < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfAxles),
                "A truck must have at least one axle.");
        }

        if (numberOfWheels < numberOfAxles * 2)
        {
            throw new ArgumentException(
                "A truck must have at least two wheels per axle.",
                nameof(numberOfWheels));
        }

        if (maximumLoadKg < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumLoadKg),
                "Maximum load cannot be negative.");
        }

        NumberOfWheels = numberOfWheels;
        NumberOfAxles = numberOfAxles;
        MaximumLoadKg = maximumLoadKg;
    }

    public int NumberOfWheels { get; }
    public int NumberOfAxles { get; }
    public double MaximumLoadKg { get; }
}
