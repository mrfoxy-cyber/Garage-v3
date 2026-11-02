using Garage.Engines;
using Garage.Factories;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using Vehicles.Interfaces;

namespace Garage.Tests.Fixtures;

internal enum FixtureKind
{
    Minimal,
    Full
}

internal static class VehicleFixtures
{
    public static Car CreateCar(FixtureKind kind = FixtureKind.Full, string registrationNumber = "ABC123", VehicleColor color = VehicleColor.Black, string manufacturer = "Volvo", int manufacturingYear = 2020, Engine? engine = null, CountryCode countryCode = CountryCode.SWE)
    {
        ValidateKind(kind);

        if (kind == FixtureKind.Minimal)
            return CarFactory.CreateCar(registrationNumber, countryCode);

        return CarFactory.CreateCar(
            registrationNumber: registrationNumber,
            countryCode: countryCode,
            manufacturer: manufacturer,
            model: "V70",
            manufacturingYear: manufacturingYear,
            color: color,
            weightKg: 1700,
            length: 4.8,
            width: 1.8,
            height: 1.5,
            vin: $"VIN-{registrationNumber}",
            bodyType: BodyType.Sedan,
            numberOfDoors: 5,
            numberOfWheels: 4,
            numberOfSeats: 4,
            crewSeats: 1,
            engines: [engine ?? EngineFixtures.CreateEngine(serialNumber: $"ENG-{registrationNumber}")]);
    }

    public static Motorcycle CreateMotorcycle(FixtureKind kind = FixtureKind.Full, string registrationNumber = "MCY123", VehicleColor color = VehicleColor.Red, int numberOfWheels = 2, Engine? engine = null, CountryCode countryCode = CountryCode.SWE)
    {
        ValidateKind(kind);

        if (kind == FixtureKind.Minimal)
            return MotorcycleFactory.CreateMotorcycle(registrationNumber, countryCode);

        return MotorcycleFactory.CreateMotorcycle(
            registrationNumber: registrationNumber,
            countryCode: countryCode,
            manufacturer: "Honda",
            model: "Gold Wing",
            manufacturingYear: 2022,
            color: color,
            weightKg: 390,
            lengthMeters: 2.6,
            widthMeters: 0.9,
            heightMeters: 1.4,
            vin: $"VIN-{registrationNumber}",
            motorcycleType: MotorcycleType.Touring,
            hasSidecar: false,
            numberOfWheels: numberOfWheels,
            passengerCapacity: 1,
            driverSeats: 1,
            engines: [engine ?? EngineFixtures.CreateEngine(
                serialNumber: $"ENG-{registrationNumber}",
                model: "Motorcycle inline twin",
                horsePower: 90,
                displacementCc: 1000,
                cylinderCount: 2)]);
    }

    public static Bus CreateBus(FixtureKind kind = FixtureKind.Full, int passengerSeats = 30, int standingCapacity = 20, int driverSeats = 1, Engine? engine = null, string registrationNumber = "BUS123", CountryCode countryCode = CountryCode.SWE)
    {
        ValidateKind(kind);

        if (kind == FixtureKind.Minimal)
            return BusFactory.CreateBus(registrationNumber, countryCode);

        return BusFactory.CreateBus(
            registrationNumber: registrationNumber,
            countryCode: countryCode,
            manufacturer: "Scania",
            model: "Citywide",
            manufacturingYear: 2023,
            color: VehicleColor.Blue,
            weightKg: 12000,
            lengthMeters: 12,
            widthMeters: 2.55,
            heightMeters: 3.2,
            vin: $"VIN-{registrationNumber}",
            numberOfDoors: 3,
            numberOfWheels: 6,
            passengerSeatCount: passengerSeats,
            standingPassengerCapacity: standingCapacity,
            driverSeatCount: driverSeats,
            engines: [engine ?? EngineFixtures.CreateEngine(
                serialNumber: $"ENG-{registrationNumber}",
                model: "Bus inline 6",
                fuelType: FuelType.Diesel,
                horsePower: 300,
                displacementCc: 9000,
                cylinderCount: 6)]);
    }

    public static Airplane CreateAirplane(FixtureKind kind = FixtureKind.Full, int pilotSeats = 2, bool hasDualControls = true, Engine? engine = null, string registrationNumber = "SE-ABC", CountryCode countryCode = CountryCode.SWE)
    {
        ValidateKind(kind);

        if (kind == FixtureKind.Minimal)
            return AirplaneFactory.CreateAirplane(registrationNumber, countryCode);

        return AirplaneFactory.CreateAirplane(
            registrationNumber: registrationNumber,
            countryCode: countryCode,
            manufacturer: "Cessna",
            model: "172",
            manufacturingYear: 2021,
            color: VehicleColor.White,
            weightKg: 767,
            numberOfWheels: 3,
            lengthMeters: 8.3,
            wingspanMeters: 11,
            heightMeters: 2.7,
            airframeSerialNumber: "AIRFRAME-123",
            passengerSeatCount: 2,
            crewSeatCount: 0,
            pilotSeatCount: pilotSeats,
            hasDualControls: hasDualControls,
            maximumAltitudeMeters: 4200,
            engines: [engine ?? EngineFixtures.CreateFlatEngine(serialNumber: "ENG-AIR123")]);
    }

    public static Boat CreateBoat(FixtureKind kind = FixtureKind.Full, int passengerCapacity = 8, int cabinPassengerSeats = 4, Engine? engine = null, string registrationNumber = "SFB-2345", CountryCode countryCode = CountryCode.SWE)
    {
        ValidateKind(kind);

        if (kind == FixtureKind.Minimal)
            return BoatFactory.CreateBoat(registrationNumber, countryCode);

        return BoatFactory.CreateBoat(
            registrationNumber: registrationNumber,
            countryCode: countryCode,
            manufacturer: "Nimbus",
            model: "T11",
            manufacturingYear: 2022,
            color: VehicleColor.White,
            weightKg: 5600,
            lengthMeters: 12.4,
            widthMeters: 3.5,
            heightMeters: 3.2,
            hin: "HIN-SEA123",
            draftMeters: 0.9,
            boatType: BoatType.Motorboat,
            passengerCapacity: passengerCapacity,
            driverSeats: 1,
            hasCabin: true,
            cabinPassengerSeatCount: cabinPassengerSeats,
            cabinCrewSeatCount: 1,
            engines: [engine ?? EngineFixtures.CreateEngine(
                serialNumber: "ENG-SEA123",
                model: "Marine inline 6",
                fuelType: FuelType.Diesel,
                horsePower: 400,
                displacementCc: 6000,
                cylinderCount: 6)]);
    }

    public static Truck CreateTruck(FixtureKind kind = FixtureKind.Full, int numberOfWheels = 6, int numberOfAxles = 3, double maximumLoadKg = 25000, Engine? engine = null, string registrationNumber = "TRK123", CountryCode countryCode = CountryCode.SWE)
    {
        ValidateKind(kind);

        if (kind == FixtureKind.Minimal)
            return TruckFactory.CreateTruck(registrationNumber, countryCode);

        return TruckFactory.CreateTruck(
            registrationNumber: registrationNumber,
            countryCode: countryCode,
            manufacturer: "Volvo",
            model: "FH16",
            manufacturingYear: 2023,
            color: VehicleColor.Red,
            weightKg: 9000,
            lengthMeters: 7.1,
            widthMeters: 2.5,
            heightMeters: 3.8,
            vin: $"VIN-{registrationNumber}",
            numberOfWheels: numberOfWheels,
            numberOfAxles: numberOfAxles,
            maximumLoadKg: maximumLoadKg,
            engines: [engine ?? EngineFixtures.CreateEngine(
                serialNumber: $"ENG-{registrationNumber}",
                model: "Truck inline 6",
                fuelType: FuelType.Diesel,
                horsePower: 600,
                displacementCc: 16000,
                cylinderCount: 6)]);
    }

    public static IReadOnlyList<IVehicle> CreateAll(FixtureKind kind = FixtureKind.Full)
    {
        return [CreateCar(kind), CreateMotorcycle(kind), CreateBus(kind), CreateAirplane(kind), CreateBoat(kind), CreateTruck(kind)];
    }

    private static void ValidateKind(FixtureKind kind)
    {
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Choose Minimal or Full.");
    }
}
