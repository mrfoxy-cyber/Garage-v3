using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class MotorcycleFactory
{
    public static Motorcycle CreateMotorcycle(string registrationNumber, CountryCode countryCode, string? manufacturer = null, string? model = null, int? manufacturingYear = null, VehicleColor? color = null, double? weightKg = null, double? lengthMeters = null, double? widthMeters = null, double? heightMeters = null, string? vin = null, MotorcycleType? motorcycleType = null, bool? hasSidecar = null, int? numberOfWheels = null, int? passengerCapacity = null, int? driverSeats = null, List<Engine>? engines = null)
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
                Dimensions = (lengthMeters, widthMeters, heightMeters) switch
                {
                    (null, null, null) => null,
                    (double length, double width, double height) => new Dimensions(length, width, height),
                    _ => throw new ArgumentException("Supply all three dimensions, or leave all three blank.")
                },
                Engines = engines ?? []
            }
        };
    }
}