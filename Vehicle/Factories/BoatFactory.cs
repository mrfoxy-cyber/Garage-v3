using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class BoatFactory
{
    public static Boat CreateBoat(
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
        string hin,
        double draftMeters,
        BoatType boatType,
        int passengerCapacity,
        int driverSeats,
        bool hasCabin,
        int cabinPassengerSeatCount,
        int cabinCrewSeatCount,
        List<Engine> engines)
    {
        Cabin? cabin = hasCabin
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
                Dimensions = new Dimensions(lengthMeters, widthMeters, heightMeters),
                Engines = engines ?? []
            }
        };
    }

    public static Boat CreateBoat(
    string registrationNumber,
    CountryCode countryCode,
    BoatSpecification? specification = null)
    {
        return new Boat
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Specification = specification
        };
    }
}
