using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class BusFactory
{
    public static Bus CreateBus(
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
        int numberOfDoors,
        int numberOfWheels,
        int passengerSeatCount,
        int standingPassengerCapacity,
        int driverSeatCount,
        List<Engine> engines)
    {
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
                numberOfDoors,
                numberOfWheels,
                standingPassengerCapacity,
                new Cabin
                {
                    PassengerSeatCount = passengerSeatCount,
                    CrewSeatCount = driverSeatCount
                })
            {
                WeightKg = weightKg,
                Dimensions = new Dimensions(lengthMeters, widthMeters, heightMeters),
                Engines = engines ?? []
            }
        };
    }

    public static Bus CreateBus(
    string registrationNumber,
    CountryCode countryCode,
    BusSpecification? specification = null)
    {
        return new Bus
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Specification = specification
        };
    }
}
