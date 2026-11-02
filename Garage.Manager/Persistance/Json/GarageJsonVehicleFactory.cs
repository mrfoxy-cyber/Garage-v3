using Garage.Factories;
using Garage.Vehicles;

namespace Garage.Manager.Persistence.Json;

internal static class GarageJsonVehicleFactory
{
    public static Vehicle Create(VehicleFileData data)
    {
        if (string.IsNullOrWhiteSpace(data.VehicleType))
            throw new ArgumentException("Every vehicle must specify a vehicle type.");

        if (data.VehicleType.Trim().Equals("Boat", StringComparison.OrdinalIgnoreCase) && data.NumberOfWheels is not null)
            throw new ArgumentException("Leave wheel count unspecified for boats.");

        return data.VehicleType.Trim().ToUpperInvariant() switch
        {
            "CAR" => CarFactory.CreateCar(
                registrationNumber: data.RegistrationNumber,
                countryCode: data.CountryCode,
                manufacturer: data.Manufacturer,
                model: data.Model,
                manufacturingYear: data.ManufacturingYear,
                color: data.Color,
                numberOfWheels: data.NumberOfWheels),

            "MOTORCYCLE" => MotorcycleFactory.CreateMotorcycle(
                registrationNumber: data.RegistrationNumber,
                countryCode: data.CountryCode,
                manufacturer: data.Manufacturer,
                model: data.Model,
                manufacturingYear: data.ManufacturingYear,
                color: data.Color,
                numberOfWheels: data.NumberOfWheels),

            "BUS" => BusFactory.CreateBus(
                registrationNumber: data.RegistrationNumber,
                countryCode: data.CountryCode,
                manufacturer: data.Manufacturer,
                model: data.Model,
                manufacturingYear: data.ManufacturingYear,
                color: data.Color,
                numberOfWheels: data.NumberOfWheels),

            "AIRPLANE" => AirplaneFactory.CreateAirplane(
                registrationNumber: data.RegistrationNumber,
                countryCode: data.CountryCode,
                manufacturer: data.Manufacturer,
                model: data.Model,
                manufacturingYear: data.ManufacturingYear,
                color: data.Color,
                numberOfWheels: data.NumberOfWheels),

            "BOAT" => BoatFactory.CreateBoat(
                registrationNumber: data.RegistrationNumber,
                countryCode: data.CountryCode,
                manufacturer: data.Manufacturer,
                model: data.Model,
                manufacturingYear: data.ManufacturingYear,
                color: data.Color),

            _ => throw new ArgumentException($"Unsupported vehicle type '{data.VehicleType}'.")
        };
    }
}
