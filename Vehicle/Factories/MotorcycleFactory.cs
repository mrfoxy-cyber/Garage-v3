using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class MotorcycleFactory
{
    public static Motorcycle CreateMotorcycle(
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
        MotorcycleType motorcycleType,
        bool hasSidecar,
        int numberOfWheels,
        int passengerCapacity,
        int driverSeats,
        List<Engine> engines)
    {
        return new Motorcycle
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            Color = color,
            Vin = vin,
            Specification = new MotorcycleSpecification(
                                    motorcycleType: motorcycleType,
                                    hasSidecar: hasSidecar,
                                    numberOfWheels: numberOfWheels,
                                    passengerCapacity: passengerCapacity,
                                    driverSeats: driverSeats)
            {
                WeightKg = weightKg,
                Dimensions = new Dimensions(
                                    lengthMeters, widthMeters, heightMeters),
                Engines = engines ?? []
            }
        };
    }

    public static Motorcycle CreateMotorcycle(
    string registrationNumber,
    CountryCode countryCode,
    MotorcycleSpecification? specification = null)
    {
        return new Motorcycle
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Specification = specification
        };
    }
}
