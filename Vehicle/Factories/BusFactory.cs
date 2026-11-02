using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class BusFactory
{
    public static Bus CreateBus(string registrationNumber, CountryCode countryCode, string? manufacturer = null, string? model = null, int? manufacturingYear = null, VehicleColor? color = null, double? weightKg = null, double? lengthMeters = null, double? widthMeters = null, double? heightMeters = null, string? vin = null, int? numberOfDoors = null, int? numberOfWheels = null, int? passengerSeatCount = null, int? standingPassengerCapacity = null, int? driverSeatCount = null, List<Engine>? engines = null)
    {
        Cabin? cabin = passengerSeatCount is null && driverSeatCount is null
            ? null
            : new Cabin
            {
                PassengerSeatCount = passengerSeatCount,
                CrewSeatCount = driverSeatCount
            };

        return new Bus
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            Color = color,
            Vin = vin,
            Specification = new BusSpecification(
                numberOfDoors: numberOfDoors,
                numberOfWheels: numberOfWheels,
                standingPassengerCapacity: standingPassengerCapacity,
                cabin: cabin)
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