using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class BoatFactory
{
    public static Boat CreateBoat(string registrationNumber, CountryCode countryCode, string? manufacturer = null, string? model = null, int? manufacturingYear = null, VehicleColor? color = null, double? weightKg = null, double? lengthMeters = null, double? widthMeters = null, double? heightMeters = null, string? hin = null, double? draftMeters = null, BoatType? boatType = null, int? passengerCapacity = null, int? driverSeats = null, bool? hasCabin = null, int? cabinPassengerSeatCount = null, int? cabinCrewSeatCount = null, List<Engine>? engines = null)
    {
        Cabin? cabin = hasCabin == true
            ? new Cabin
            {
                PassengerSeatCount = cabinPassengerSeatCount,
                CrewSeatCount = cabinCrewSeatCount
            }
            : null;

        return new Boat
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            Color = color,
            Hin = hin,
            Specification = new BoatSpecification(
                passengerCapacity: passengerCapacity,
                driverSeats: driverSeats,
                cabin: cabin,
                draftMeters: draftMeters,
                boatType: boatType)
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