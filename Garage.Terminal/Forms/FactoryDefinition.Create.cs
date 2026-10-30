using Garage.Engines;
using Garage.Factories;
using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Terminal.Forms;

internal sealed partial class FactoryDefinition
{
    public object Create(Dictionary<string, object?> values, List<Engine>? engines = null) =>
        methodName switch
        {
            "CreateCar" => CarFactory.CreateCar(
                (string)values["registrationNumber"]!,
                (CountryCode)values["countryCode"]!,
                (string)values["manufacturer"]!,
                (string)values["model"]!,
                (int)values["manufacturingYear"]!,
                (VehicleColor)values["color"]!,
                (double)values["weightKg"]!,
                (double)values["length"]!,
                (double)values["width"]!,
                (double)values["height"]!,
                (string)values["vin"]!,
                (BodyType)values["bodyType"]!,
                (int)values["numberOfDoors"]!,
                (int)values["numberOfSeats"]!,
                engines ?? []
            ),
            "CreateMotorcycle" => MotorcycleFactory.CreateMotorcycle(
                (string)values["registrationNumber"]!,
                (CountryCode)values["countryCode"]!,
                (string)values["manufacturer"]!,
                (string)values["model"]!,
                (int)values["manufacturingYear"]!,
                (VehicleColor)values["color"]!,
                (double)values["weightKg"]!,
                (double)values["lengthMeters"]!,
                (double)values["widthMeters"]!,
                (double)values["heightMeters"]!,
                (string)values["vin"]!,
                (MotorcycleType)values["motorcycleType"]!,
                (bool)values["hasSidecar"]!,
                (int)values["numberOfWheels"]!,
                (int)values["passengerCapacity"]!,
                (int)values["driverSeats"]!,
                engines ?? []
            ),
            "CreateAirplane" => AirplaneFactory.CreateAirplane(
                (string)values["registrationNumber"]!,
                (CountryCode)values["countryCode"]!,
                (string)values["manufacturer"]!,
                (string)values["model"]!,
                (int)values["manufacturingYear"]!,
                (VehicleColor)values["color"]!,
                (double)values["weightKg"]!,
                (int)values["numberOfWheels"]!,
                (double)values["lengthMeters"]!,
                (double)values["wingspanMeters"]!,
                (double)values["heightMeters"]!,
                (string)values["airframeSerialNumber"]!,
                (int)values["passengerSeatCount"]!,
                (int)values["crewSeatCount"]!,
                (int)values["pilotSeatCount"]!,
                (bool)values["hasDualControls"]!,
                (double)values["maximumAltitudeMeters"]!,
                engines ?? []
            ),
            "CreateBus" => BusFactory.CreateBus(
                (string)values["registrationNumber"]!,
                (CountryCode)values["countryCode"]!,
                (string)values["manufacturer"]!,
                (string)values["model"]!,
                (int)values["manufacturingYear"]!,
                (VehicleColor)values["color"]!,
                (double)values["weightKg"]!,
                (double)values["lengthMeters"]!,
                (double)values["widthMeters"]!,
                (double)values["heightMeters"]!,
                (string)values["vin"]!,
                (int)values["numberOfDoors"]!,
                (int)values["numberOfWheels"]!,
                (int)values["passengerSeatCount"]!,
                (int)values["standingPassengerCapacity"]!,
                (int)values["driverSeatCount"]!,
                engines ?? []
            ),
            "CreateBoat" => BoatFactory.CreateBoat(
                (string)values["registrationNumber"]!,
                (CountryCode)values["countryCode"]!,
                (string)values["manufacturer"]!,
                (string)values["model"]!,
                (int)values["manufacturingYear"]!,
                (VehicleColor)values["color"]!,
                (double)values["weightKg"]!,
                (double)values["lengthMeters"]!,
                (double)values["widthMeters"]!,
                (double)values["heightMeters"]!,
                (string)values["hin"]!,
                (double)values["draftMeters"]!,
                (BoatType)values["boatType"]!,
                (int)values["passengerCapacity"]!,
                (int)values["driverSeats"]!,
                (bool)values["hasCabin"]!,
                (int)values["cabinPassengerSeatCount"]!,
                (int)values["cabinCrewSeatCount"]!,
                engines ?? []
            ),
            "CreateTruck" => TruckFactory.CreateTruck(
                (string)values["registrationNumber"]!,
                (CountryCode)values["countryCode"]!,
                (string)values["manufacturer"]!,
                (string)values["model"]!,
                (int)values["manufacturingYear"]!,
                (VehicleColor)values["color"]!,
                (double)values["weightKg"]!,
                (double)values["lengthMeters"]!,
                (double)values["widthMeters"]!,
                (double)values["heightMeters"]!,
                (string)values["vin"]!,
                (int)values["numberOfWheels"]!,
                (int)values["numberOfAxles"]!,
                (double)values["maximumLoadKg"]!,
                engines ?? []
            ),
            "CreateInlineEngine" => EngineFactory.CreateInlineEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (int)values["horsePower"]!,
                (int)values["displacementCc"]!,
                (int)values["cylinderCount"]!
            ),
            "CreateVEngine" => EngineFactory.CreateVEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (int)values["horsePower"]!,
                (int)values["displacementCc"]!,
                (int)values["cylinderCount"]!
            ),
            "CreateFlatEngine" => EngineFactory.CreateFlatEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (int)values["horsePower"]!,
                (int)values["displacementCc"]!,
                (int)values["cylinderCount"]!
            ),
            "CreateRotaryEngine" => EngineFactory.CreateRotaryEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (int)values["horsePower"]!,
                (int)values["displacementCc"]!,
                (int)values["rotorCount"]!
            ),
            "CreateElectricMotor" => EngineFactory.CreateElectricMotor(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (double)values["powerKw"]!,
                (double)values["voltage"]!
            ),
            "CreateSteamEngine" => EngineFactory.CreateSteamEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (int)values["horsePower"]!,
                (double)values["boilerPressureBar"]!
            ),
            "CreateTurbojetEngine" => EngineFactory.CreateTurbojetEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (double)values["thrustKn"]!
            ),
            "CreateTurbofanEngine" => EngineFactory.CreateTurbofanEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (double)values["thrustKn"]!,
                (double)values["bypassRatio"]!
            ),
            "CreateTurbopropEngine" => EngineFactory.CreateTurbopropEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (double)values["thrustKn"]!,
                (int)values["shaftHorsePower"]!
            ),
            "CreateTurboshaftEngine" => EngineFactory.CreateTurboshaftEngine(
                (string)values["serialNumber"]!,
                (string)values["model"]!,
                (FuelType)values["fuelType"]!,
                (double)values["thrustKn"]!,
                (int)values["shaftHorsePower"]!
            ),
            _ => throw new ArgumentException($"Unknown factory method: {methodName}.")
        };
}
