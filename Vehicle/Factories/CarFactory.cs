using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;

namespace Garage.Factories;

public static class CarFactory
{
    public static Car CreateCar(
        string registrationNumber,
        CountryCode countryCode,
        string manufacturer,
        string model,
        int manufacturingYear,
        VehicleColor color,
        double weightKg,
        double length,
        double width,
        double height,
        string vin,
        BodyType bodyType,
        int numberOfDoors,
        int numberOfSeats,
        List<Engine> engines)
    {
        return new Car
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = manufacturingYear,
            Color = color,
            Vin = vin,
            Specification = new CarSpecification(
                                    bodyType: bodyType,
                                    numberOfDoors: numberOfDoors,
                                    numberOfWheels: 4,
                                    cabin: new Cabin
            {
                PassengerSeatCount = numberOfSeats - 1,
                CrewSeatCount = 1
            })
                    {
                        WeightKg = weightKg,
                        Dimensions = new Dimensions(length, width, height),
                        Engines = engines ?? []
            }
        };
    }

    public static Car CreateCar(
    string registrationNumber,
    CountryCode countryCode,
    CarSpecification? specification = null)
    {
        return new Car
        {
            RegistrationNumber = registrationNumber,
            CountryCode = countryCode,
            Specification = specification
        };
    }
}
