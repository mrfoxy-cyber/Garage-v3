using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class AirplaneFactory
{
    public static Airplane CreateAirplane(string registrationNumber, CountryCode countryCode, string? manufacturer = null, string? model = null, int? manufacturingYear = null, VehicleColor? color = null, double? weightKg = null, int? numberOfWheels = null, double? lengthMeters = null, double? wingspanMeters = null, double? heightMeters = null, string? airframeSerialNumber = null, int? passengerSeatCount = null, int? crewSeatCount = null, int? pilotSeatCount = null, bool? hasDualControls = null, double? maximumAltitudeMeters = null, List<Engine>? engines = null)
    {
        Cabin? cabin = passengerSeatCount is null && crewSeatCount is null
            ? null
            : new Cabin
            {
                PassengerSeatCount = passengerSeatCount,
                CrewSeatCount = crewSeatCount
            };

        Cockpit? cockpit = pilotSeatCount is null && hasDualControls is null
            ? null
            : new Cockpit
            {
                PilotSeats = pilotSeatCount,
                HasDualControls = hasDualControls ?? false
            };

        return new Airplane
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            Color = color,
            AirframeSerialNumber = airframeSerialNumber,
            Specification = new AirplaneSpecification(
                cabin: cabin,
                cockpit: cockpit,
                maximumAltitudeMeters: maximumAltitudeMeters,
                wingspanMeters: wingspanMeters,
                numberOfWheels: numberOfWheels)
            {
                WeightKg = weightKg,
                Dimensions = (lengthMeters, wingspanMeters, heightMeters) switch
                {
                    (null, null, null) => null,
                    (double length, double width, double height) => new Dimensions(length, width, height),
                    _ => throw new ArgumentException("Supply length, wingspan and height together, or leave all three blank.")
                },
                Engines = engines ?? []
            }
        };
    }
}