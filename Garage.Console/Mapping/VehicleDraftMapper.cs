using Garage.ConsoleApp.Drafts;
using Garage.Factories;
using Garage.Vehicles;

namespace Garage.ConsoleApp.Mapping;

internal sealed class VehicleDraftMapper
{
    public Car CreateCar(CarDraft draft)
    {
        return CarFactory.CreateCar(
            draft.RegistrationNumber!, draft.CountryCode!.Value,
            draft.Manufacturer!, draft.Model!,
            draft.ManufacturingYear!.Value, draft.Color!.Value,
            draft.WeightKg!.Value, draft.LengthMeters!.Value,
            draft.WidthMeters!.Value, draft.HeightMeters!.Value,
            draft.Vin!, draft.BodyType!.Value,
            draft.NumberOfDoors!.Value, draft.NumberOfSeats!.Value,
            draft.Engines);
    }

    public Motorcycle CreateMotorcycle(MotorcycleDraft draft)
    {
        return MotorcycleFactory.CreateMotorcycle(
            draft.RegistrationNumber!, draft.CountryCode!.Value,
            draft.Manufacturer!, draft.Model!,
            draft.ManufacturingYear!.Value, draft.Color!.Value,
            draft.WeightKg!.Value, draft.LengthMeters!.Value,
            draft.WidthMeters!.Value, draft.HeightMeters!.Value,
            draft.Vin!, draft.MotorcycleType!.Value,
            draft.HasSidecar!.Value, draft.NumberOfWheels!.Value,
            draft.PassengerCapacity!.Value, draft.DriverSeats!.Value,
            draft.Engines);
    }

    public Airplane CreateAirplane(AirplaneDraft draft)
    {
        return AirplaneFactory.CreateAirplane(
            draft.RegistrationNumber!, draft.CountryCode!.Value,
            draft.Manufacturer!, draft.Model!,
            draft.ManufacturingYear!.Value, draft.Color!.Value,
            draft.WeightKg!.Value, draft.NumberOfWheels!.Value,
            draft.LengthMeters!.Value, draft.WingspanMeters!.Value,
            draft.HeightMeters!.Value, draft.AirframeSerialNumber!,
            draft.PassengerSeats!.Value, draft.CrewSeats!.Value,
            draft.PilotSeats!.Value, draft.HasDualControls!.Value,
            draft.MaximumAltitudeMeters!.Value, draft.Engines);
    }

    public Bus CreateBus(BusDraft draft)
    {
        return BusFactory.CreateBus(
            draft.RegistrationNumber!, draft.CountryCode!.Value,
            draft.Manufacturer!, draft.Model!,
            draft.ManufacturingYear!.Value, draft.Color!.Value,
            draft.WeightKg!.Value, draft.LengthMeters!.Value,
            draft.WidthMeters!.Value, draft.HeightMeters!.Value,
            draft.Vin!, draft.NumberOfDoors!.Value,
            draft.NumberOfWheels!.Value, draft.PassengerSeats!.Value,
            draft.StandingPassengerCapacity!.Value,
            draft.DriverSeats!.Value, draft.Engines);
    }

    public Boat CreateBoat(BoatDraft draft)
    {
        return BoatFactory.CreateBoat(
            draft.RegistrationNumber!, draft.CountryCode!.Value,
            draft.Manufacturer!, draft.Model!,
            draft.ManufacturingYear!.Value, draft.Color!.Value,
            draft.WeightKg!.Value, draft.LengthMeters!.Value,
            draft.WidthMeters!.Value, draft.HeightMeters!.Value,
            draft.Hin!, draft.DraftMeters!.Value, draft.BoatType!.Value,
            draft.PassengerCapacity!.Value, draft.DriverSeats!.Value,
            draft.HasCabin!.Value, draft.CabinPassengerSeats ?? 0,
            draft.CabinCrewSeats ?? 0, draft.Engines);
    }

    public Truck CreateTruck(TruckDraft draft)
    {
        return TruckFactory.CreateTruck(
            draft.RegistrationNumber!, draft.CountryCode!.Value,
            draft.Manufacturer!, draft.Model!,
            draft.ManufacturingYear!.Value, draft.Color!.Value,
            draft.WeightKg!.Value, draft.LengthMeters!.Value,
            draft.WidthMeters!.Value, draft.HeightMeters!.Value,
            draft.Vin!, draft.NumberOfWheels!.Value,
            draft.NumberOfAxles!.Value, draft.MaximumLoadKg!.Value,
            draft.Engines);
    }

}
