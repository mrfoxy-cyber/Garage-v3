using Garage.Engines;
using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Terminal.Forms;

internal sealed partial class FactoryDefinition
{
    private static List<FieldDefinition> BuildFields(string methodName) => methodName switch
    {
        "CreateCar" =>
        [
            Field("registrationNumber", typeof(string)),
            Field("countryCode", typeof(CountryCode)),
            Field("manufacturer", typeof(string)),
            Field("model", typeof(string)),
            Field("manufacturingYear", typeof(int)),
            Field("color", typeof(VehicleColor)),
            Field("weightKg", typeof(double)),
            Field("length", typeof(double)),
            Field("width", typeof(double)),
            Field("height", typeof(double)),
            Field("vin", typeof(string)),
            Field("bodyType", typeof(BodyType)),
            Field("numberOfDoors", typeof(int)),
            Field("numberOfSeats", typeof(int))
        ],
        "CreateMotorcycle" =>
        [
            Field("registrationNumber", typeof(string)),
            Field("countryCode", typeof(CountryCode)),
            Field("manufacturer", typeof(string)),
            Field("model", typeof(string)),
            Field("manufacturingYear", typeof(int)),
            Field("color", typeof(VehicleColor)),
            Field("weightKg", typeof(double)),
            Field("lengthMeters", typeof(double)),
            Field("widthMeters", typeof(double)),
            Field("heightMeters", typeof(double)),
            Field("vin", typeof(string)),
            Field("motorcycleType", typeof(MotorcycleType)),
            Field("hasSidecar", typeof(bool)),
            Field("numberOfWheels", typeof(int)),
            Field("passengerCapacity", typeof(int)),
            Field("driverSeats", typeof(int))
        ],
        "CreateAirplane" =>
        [
            Field("registrationNumber", typeof(string)),
            Field("countryCode", typeof(CountryCode)),
            Field("manufacturer", typeof(string)),
            Field("model", typeof(string)),
            Field("manufacturingYear", typeof(int)),
            Field("color", typeof(VehicleColor)),
            Field("weightKg", typeof(double)),
            Field("numberOfWheels", typeof(int)),
            Field("lengthMeters", typeof(double)),
            Field("wingspanMeters", typeof(double)),
            Field("heightMeters", typeof(double)),
            Field("airframeSerialNumber", typeof(string)),
            Field("passengerSeatCount", typeof(int)),
            Field("crewSeatCount", typeof(int)),
            Field("pilotSeatCount", typeof(int)),
            Field("hasDualControls", typeof(bool)),
            Field("maximumAltitudeMeters", typeof(double))
        ],
        "CreateBus" =>
        [
            Field("registrationNumber", typeof(string)),
            Field("countryCode", typeof(CountryCode)),
            Field("manufacturer", typeof(string)),
            Field("model", typeof(string)),
            Field("manufacturingYear", typeof(int)),
            Field("color", typeof(VehicleColor)),
            Field("weightKg", typeof(double)),
            Field("lengthMeters", typeof(double)),
            Field("widthMeters", typeof(double)),
            Field("heightMeters", typeof(double)),
            Field("vin", typeof(string)),
            Field("numberOfDoors", typeof(int)),
            Field("numberOfWheels", typeof(int)),
            Field("passengerSeatCount", typeof(int)),
            Field("standingPassengerCapacity", typeof(int)),
            Field("driverSeatCount", typeof(int))
        ],
        "CreateBoat" =>
        [
            Field("registrationNumber", typeof(string)),
            Field("countryCode", typeof(CountryCode)),
            Field("manufacturer", typeof(string)),
            Field("model", typeof(string)),
            Field("manufacturingYear", typeof(int)),
            Field("color", typeof(VehicleColor)),
            Field("weightKg", typeof(double)),
            Field("lengthMeters", typeof(double)),
            Field("widthMeters", typeof(double)),
            Field("heightMeters", typeof(double)),
            Field("hin", typeof(string)),
            Field("draftMeters", typeof(double)),
            Field("boatType", typeof(BoatType)),
            Field("passengerCapacity", typeof(int)),
            Field("driverSeats", typeof(int)),
            Field("hasCabin", typeof(bool)),
            Field("cabinPassengerSeatCount", typeof(int)),
            Field("cabinCrewSeatCount", typeof(int))
        ],
        "CreateTruck" =>
        [
            Field("registrationNumber", typeof(string)),
            Field("countryCode", typeof(CountryCode)),
            Field("manufacturer", typeof(string)),
            Field("model", typeof(string)),
            Field("manufacturingYear", typeof(int)),
            Field("color", typeof(VehicleColor)),
            Field("weightKg", typeof(double)),
            Field("lengthMeters", typeof(double)),
            Field("widthMeters", typeof(double)),
            Field("heightMeters", typeof(double)),
            Field("vin", typeof(string)),
            Field("numberOfWheels", typeof(int)),
            Field("numberOfAxles", typeof(int)),
            Field("maximumLoadKg", typeof(double))
        ],
        "CreateInlineEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("horsePower", typeof(int)),
            Field("displacementCc", typeof(int)),
            Field("cylinderCount", typeof(int))
        ],
        "CreateVEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("horsePower", typeof(int)),
            Field("displacementCc", typeof(int)),
            Field("cylinderCount", typeof(int))
        ],
        "CreateFlatEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("horsePower", typeof(int)),
            Field("displacementCc", typeof(int)),
            Field("cylinderCount", typeof(int))
        ],
        "CreateRotaryEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("horsePower", typeof(int)),
            Field("displacementCc", typeof(int)),
            Field("rotorCount", typeof(int))
        ],
        "CreateElectricMotor" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("powerKw", typeof(double)),
            Field("voltage", typeof(double))
        ],
        "CreateSteamEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("horsePower", typeof(int)),
            Field("boilerPressureBar", typeof(double))
        ],
        "CreateTurbojetEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("thrustKn", typeof(double))
        ],
        "CreateTurbofanEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("thrustKn", typeof(double)),
            Field("bypassRatio", typeof(double))
        ],
        "CreateTurbopropEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("thrustKn", typeof(double)),
            Field("shaftHorsePower", typeof(int))
        ],
        "CreateTurboshaftEngine" =>
        [
            Field("serialNumber", typeof(string)),
            Field("model", typeof(string)),
            Field("fuelType", typeof(FuelType)),
            Field("thrustKn", typeof(double)),
            Field("shaftHorsePower", typeof(int))
        ],
        _ => throw new ArgumentException($"Unknown factory method: {methodName}.")
    };
}
