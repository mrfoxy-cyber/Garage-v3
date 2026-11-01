using Garage.Factories;
using Garage.Terminal.Contracts;
using Garage.Vehicles;
using Garage.Vehicles.Specifications;
using Garage_V3.Interfaces;
using Garage_V3.results;

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

        Vehicle vehicle = request.VehicleType switch
        {
            "Car" => CarFactory.CreateCar(
                request.RegistrationNumber,
                request.CountryCode,
                wheels is null ? null : new CarSpecification(numberOfWheels: wheels)),

            "Motorcycle" => MotorcycleFactory.CreateMotorcycle(
                request.RegistrationNumber,
                request.CountryCode,
                wheels is null ? null : new MotorcycleSpecification(numberOfWheels: wheels)),

            "Bus" => BusFactory.CreateBus(
                request.RegistrationNumber,
                request.CountryCode,
                wheels is null ? null : new BusSpecification(numberOfWheels: wheels)),

            "Airplane" => AirplaneFactory.CreateAirplane(
                request.RegistrationNumber,
                request.CountryCode,
                wheels is null ? null : new AirplaneSpecification(numberOfWheels: wheels)),

            "Boat" => BoatFactory.CreateBoat(
                request.RegistrationNumber,
                request.CountryCode),

            _ => throw new ArgumentException("Choose a supported vehicle type.")
        };

        vehicle.Color = request.Color;

        return vehicle;
    }
}
