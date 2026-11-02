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
        string? manufacturer=null,
        string? model=null,
        int? manufacturingYear=null,
        int? numberOfWheels=null,
        VehicleColor? color= null,
        double? weightKg=null,
        double? length=null,
        double? width=null,
        double? height=null,
        string? vin=null,
        BodyType bodyType = BodyType.Unknown,
        int? numberOfDoors = null,
        int? numberOfSeats=null,
        int? crewSeats=null,
        List<Engine>? engines = null)
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
                                    numberOfWheels: numberOfWheels,
                                    cabin: new Cabin
            {
                PassengerSeatCount = numberOfSeats,
                CrewSeatCount = crewSeats
                                    })
                    {
                        WeightKg = weightKg,
                        Dimensions = (length, width, height) switch
                        {
                            (null, null, null) => null,
                            (double l, double w, double h) => new Dimensions(l, w, h),
                            _ => throw new ArgumentException("Supply all three dimensions, or leave all three blank.")
                        },
                        Engines = engines ?? []
            }
        };
    }
}
