using Garage.Factories;
using Garage.Terminal.Contracts;
using Garage.Vehicles;
using Garage.Vehicles.Specifications;
using Garage_V3.Interfaces;
using Garage_V3.results;
using Vehicles.Interfaces;

namespace Garage.Terminal.Services;

public sealed class ParkService
{
    private readonly IHandler handler;

    public ParkService(IHandler handler)
    {
        this.handler = handler;
    }

    public ParkingResult ParkVehicle(ParkVehicleRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            Vehicle vehicle = CreateVehicle(request);

            return handler.ParkVehicle(vehicle, request.SpotNumber);
        }
        catch (ArgumentException exception)
        {
            return ParkingResult.Failed(exception.Message, OperationError.ValidationFailed);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);

            return ParkingResult.Failed(
                "Could not park the vehicle. Please try again.",
                OperationError.UnexpectedError);
        }
    }

    private static Vehicle CreateVehicle(ParkVehicleRequest request)
    {
        int? wheels = request.NumberOfWheels;

        if (request.VehicleType == "Boat" && wheels is not null)
            throw new ArgumentException("Leave wheel count blank for boats.");

        return request.VehicleType switch
        {
            "Car" => CarFactory.CreateCar(
                registrationNumber: request.RegistrationNumber,
                countryCode: request.CountryCode,
                manufacturer: request.Manufacturer,
                model: request.Model,
                manufacturingYear: request.ManufacturingYear,
                color: request.Color,
                numberOfWheels: request.NumberOfWheels),

            "Motorcycle" => MotorcycleFactory.CreateMotorcycle(
                registrationNumber: request.RegistrationNumber,
                countryCode: request.CountryCode,
                manufacturer: request.Manufacturer,
                model: request.Model,
                manufacturingYear: request.ManufacturingYear,
                color: request.Color,
                numberOfWheels: request.NumberOfWheels),

            "Bus" => BusFactory.CreateBus(
                registrationNumber: request.RegistrationNumber,
                countryCode: request.CountryCode,
                manufacturer: request.Manufacturer,
                model: request.Model,
                manufacturingYear: request.ManufacturingYear,
                color: request.Color,
                numberOfWheels: request.NumberOfWheels),

            "Airplane" => AirplaneFactory.CreateAirplane(
                registrationNumber: request.RegistrationNumber,
                countryCode: request.CountryCode,
                manufacturer: request.Manufacturer,
                model: request.Model,
                manufacturingYear: request.ManufacturingYear,
                color: request.Color,
                numberOfWheels: request.NumberOfWheels),

            "Boat" => BoatFactory.CreateBoat(
                registrationNumber: request.RegistrationNumber,
                countryCode: request.CountryCode,
                manufacturer: request.Manufacturer,
                model: request.Model,
                manufacturingYear: request.ManufacturingYear,
                color: request.Color),

            _ => throw new ArgumentException("Choose a supported vehicle type.")
        };
    }
}
