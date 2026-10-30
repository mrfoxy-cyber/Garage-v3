using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class TruckFactory
{
    public static Truck CreateTruck(
        string registrationNumber,
        CountryCode countryCode,
        string manufacturer,
        string model,
        int manufacturingYear,
        VehicleColor color,
        double weightKg,
        double lengthMeters,
        double widthMeters,
        double heightMeters,
        string vin,
        int numberOfWheels,
        int numberOfAxles,
        double maximumLoadKg,
        List<Engine> engines)
    {
        return new Truck
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            Color = color,
            Vin = vin,

            Specification = new TruckSpecification(
                numberOfWheels,
                numberOfAxles,
                maximumLoadKg)
            {
                WeightKg = weightKg,

                Dimensions = new Dimensions(
                    lengthMeters,
                    widthMeters,
                    heightMeters),

                Engines = engines ?? []
            }
        };
    }

    public static Truck CreateTruck(
        string registrationNumber,
        CountryCode countryCode,
        TruckSpecification? specification =null)
    {
        return new Truck
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Specification = specification
        };
    }
}
