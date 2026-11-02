using Garage.Engines;
using Garage.Engines.InternalCombustion.Piston;
using Garage.Engines.Specifications;
using Garage.Factories;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;
using Vehicles.Interfaces;
using Xunit;

namespace Garage.Tests;

public sealed class VehicleFactoryTests
{
    [Fact]
    public void CreateMotorcycle_AllDetailsProvided_PreservesDetailsAndCalculatesTotalSeats()
    {
        // Arrange
        Engine engine = CreateEngine("ENG-MOTORCYCLE");

        // Act
        Motorcycle vehicle = MotorcycleFactory.CreateMotorcycle(
            registrationNumber: "MCY123", countryCode: CountryCode.SWE,
            manufacturer: "Honda", model: "Gold Wing", manufacturingYear: 2022,
            color: VehicleColor.Red, weightKg: 390,
            lengthMeters: 2.6, widthMeters: 0.9, heightMeters: 1.4,
            vin: "VIN-MCY123", motorcycleType: MotorcycleType.Touring,
            hasSidecar: true, numberOfWheels: 3, passengerCapacity: 2,
            driverSeats: 1, engines: [engine]);

        // Assert
        AssertCommon(vehicle, "MCY123", "Honda", "Gold Wing", 2022, VehicleColor.Red, 390, new Dimensions(2.6, 0.9, 1.4), engine);
        var specification = Assert.IsType<MotorcycleSpecification>(vehicle.Specification);
        Assert.Same(specification, vehicle.SpecificationDetails);
        Assert.Equal("VIN-MCY123", vehicle.Vin);
        Assert.Equal(MotorcycleType.Touring, specification.MotorcycleType);
        Assert.Equal(true, specification.HasSidecar);
        Assert.Equal(3, specification.NumberOfWheels);
        Assert.Equal(3, vehicle.NumberOfWheels);
        Assert.Equal(2, specification.PassengerCapacity);
        Assert.Equal(2, vehicle.PassengerCapacity);
        Assert.Equal(1, specification.DriverSeats);
        Assert.Equal(1, vehicle.DriverSeats);
        Assert.Equal(3, vehicle.NumberOfSeats);
    }

    [Fact]
    public void CreateBoat_AllDetailsProvided_PreservesCabinCapacityDraftAndDimensions()
    {
        // Arrange
        Engine engine = CreateEngine("ENG-BOAT", FuelType.Diesel);

        // Act
        Boat vehicle = BoatFactory.CreateBoat(
            registrationNumber: "SABC", countryCode: CountryCode.SWE,
            manufacturer: "Nimbus", model: "T11", manufacturingYear: 2022,
            color: VehicleColor.White, weightKg: 5600,
            lengthMeters: 12.4, widthMeters: 3.5, heightMeters: 3.2,
            hin: "HIN-SEA123", draftMeters: 0.9, boatType: BoatType.Motorboat,
            passengerCapacity: 8, driverSeats: 1, hasCabin: true,
            cabinPassengerSeatCount: 4, cabinCrewSeatCount: 1, engines: [engine]);

        // Assert
        AssertCommon(vehicle, "SABC", "Nimbus", "T11", 2022, VehicleColor.White, 5600, new Dimensions(12.4, 3.5, 3.2), engine);
        var specification = Assert.IsType<BoatSpecification>(vehicle.Specification);
        var cabin = Assert.IsType<Cabin>(specification.Cabin);
        Assert.Same(specification, vehicle.SpecificationDetails);
        Assert.Equal("HIN-SEA123", vehicle.Hin);
        Assert.Equal(BoatType.Motorboat, specification.BoatType);
        Assert.Equal(0.9, specification.DraftMeters);
        Assert.Equal(8, specification.PassengerCapacity);
        Assert.Equal(8, vehicle.PassengerCapacity);
        Assert.Equal(1, specification.DriverSeats);
        Assert.Equal(1, vehicle.DriverSeats);
        Assert.Equal(true, vehicle.HasCabin);
        Assert.Equal(4, cabin.PassengerSeatCount);
        Assert.Equal(1, cabin.CrewSeatCount);
        Assert.Equal(9, vehicle.NumberOfSeats);
    }

    [Fact]
    public void CreateBus_AllDetailsProvided_IncludesStandingPassengersButNotInSeatCount()
    {
        // Arrange
        Engine engine = CreateEngine("ENG-BUS", FuelType.Diesel);

        // Act
        Bus vehicle = BusFactory.CreateBus(
            registrationNumber: "BUS123", countryCode: CountryCode.SWE,
            manufacturer: "Scania", model: "Citywide", manufacturingYear: 2023,
            color: VehicleColor.Blue, weightKg: 12000,
            lengthMeters: 12, widthMeters: 2.55, heightMeters: 3.2,
            vin: "VIN-BUS123", numberOfDoors: 3, numberOfWheels: 6,
            passengerSeatCount: 30, standingPassengerCapacity: 20,
            driverSeatCount: 1, engines: [engine]);

        // Assert
        AssertCommon(vehicle, "BUS123", "Scania", "Citywide", 2023, VehicleColor.Blue, 12000, new Dimensions(12, 2.55, 3.2), engine);
        var specification = Assert.IsType<BusSpecification>(vehicle.Specification);
        var cabin = Assert.IsType<Cabin>(specification.Cabin);
        Assert.Same(specification, vehicle.SpecificationDetails);
        Assert.Equal("VIN-BUS123", vehicle.Vin);
        Assert.Equal(3, specification.NumberOfDoors);
        Assert.Equal(6, specification.NumberOfWheels);
        Assert.Equal(6, vehicle.NumberOfWheels);
        Assert.Equal(30, cabin.PassengerSeatCount);
        Assert.Equal(1, cabin.CrewSeatCount);
        Assert.Equal(20, specification.StandingPassengerCapacity);
        Assert.Equal(50, vehicle.PassengerCapacity);
        Assert.Equal(1, vehicle.DriverSeats);
        Assert.Equal(31, vehicle.NumberOfSeats);
    }

    [Fact]
    public void CreateAirplane_AllDetailsProvided_PreservesCockpitAndBothEngines()
    {
        // Arrange
        Engine first = CreateAircraftEngine("ENG-AIR-1");
        Engine second = CreateAircraftEngine("ENG-AIR-2");

        // Act
        Airplane vehicle = AirplaneFactory.CreateAirplane(
            registrationNumber: "SE-ABC", countryCode: CountryCode.SWE,
            manufacturer: "Example Air", model: "Test Twin", manufacturingYear: 2021,
            color: VehicleColor.Silver, weightKg: 1800, numberOfWheels: 3,
            lengthMeters: 8.3, wingspanMeters: 11, heightMeters: 2.7,
            airframeSerialNumber: "AIRFRAME-123", passengerSeatCount: 6,
            crewSeatCount: 1, pilotSeatCount: 2, hasDualControls: true,
            maximumAltitudeMeters: 4200, engines: [first, second]);

        // Assert
        AssertCommon(vehicle, "SE-ABC", "Example Air", "Test Twin", 2021, VehicleColor.Silver, 1800, new Dimensions(8.3, 11, 2.7), first, second);
        var specification = Assert.IsType<AirplaneSpecification>(vehicle.Specification);
        var cabin = Assert.IsType<Cabin>(specification.Cabin);
        var cockpit = Assert.IsType<Cockpit>(specification.Cockpit);
        Assert.Same(specification, vehicle.SpecificationDetails);
        Assert.Equal("AIRFRAME-123", vehicle.AirframeSerialNumber);
        Assert.Equal(11d, specification.WingspanMeters);
        Assert.Equal(4200d, specification.MaximumAltitudeMeters);
        Assert.Equal(3, specification.NumberOfWheels);
        Assert.Equal(3, vehicle.NumberOfWheels);
        Assert.Equal(6, cabin.PassengerSeatCount);
        Assert.Equal(1, cabin.CrewSeatCount);
        Assert.Equal(2, cockpit.PilotSeats);
        Assert.True(cockpit.HasDualControls);
        Assert.Equal(6, vehicle.PassengerCapacity);
        Assert.Equal(2, vehicle.DriverSeats);
        Assert.Equal(9, vehicle.NumberOfSeats);
    }

    [Fact]
    public void CreateCar_AllDetailsProvided_PreservesPassengerAndCrewSeatsSeparately()
    {
        // Arrange
        Engine engine = CreateEngine("ENG-CAR");

        // Act
        Car vehicle = CarFactory.CreateCar(
            registrationNumber: "ABC123", countryCode: CountryCode.SWE,
            manufacturer: "Volvo", model: "V70", manufacturingYear: 2020,
            color: VehicleColor.Black, weightKg: 1700,
            length: 4.8, width: 1.8, height: 1.5,
            vin: "VIN-ABC123", bodyType: BodyType.Sedan, numberOfDoors: 5,
            numberOfWheels: 4, numberOfSeats: 4, crewSeats: 1, engines: [engine]);

        // Assert
        AssertCommon(vehicle, "ABC123", "Volvo", "V70", 2020, VehicleColor.Black, 1700, new Dimensions(4.8, 1.8, 1.5), engine);
        var specification = Assert.IsType<CarSpecification>(vehicle.Specification);
        var cabin = Assert.IsType<Cabin>(specification.Cabin);
        Assert.Same(specification, vehicle.SpecificationDetails);
        Assert.Equal("VIN-ABC123", vehicle.Vin);
        Assert.Equal(BodyType.Sedan, specification.BodyType);
        Assert.Equal(5, specification.NumberOfDoors);
        Assert.Equal(4, specification.NumberOfWheels);
        Assert.Equal(4, vehicle.NumberOfWheels);
        Assert.Equal(4, cabin.PassengerSeatCount);
        Assert.Equal(1, cabin.CrewSeatCount);
        Assert.Equal(4, vehicle.PassengerCapacity);
        Assert.Equal(1, vehicle.DriverSeats);
        Assert.Equal(5, vehicle.NumberOfSeats);
    }

    [Theory]
    [InlineData("Car")]
    [InlineData("Motorcycle")]
    [InlineData("Bus")]
    [InlineData("Boat")]
    [InlineData("Airplane")]
    public void CreateVehicle_OptionalDetailsOmitted_LeavesDetailsUnknownAndComputedSeatsSafe(string vehicleType)
    {
        // Arrange / Act
        Vehicle vehicle = CreateMinimalVehicle(vehicleType);

        // Assert
        Assert.Null(vehicle.Manufacturer);
        Assert.Null(vehicle.Model);
        Assert.Null(vehicle.ManufacturingYear);
        Assert.Null(vehicle.Color);

        var specification = Assert.IsAssignableFrom<VehicleSpecification>(vehicle.SpecificationDetails);
        Assert.Null(specification.WeightKg);
        Assert.Null(specification.Dimensions);
        Assert.Empty(specification.Engines);

        if (vehicle is IHasWheels wheels)
            Assert.Null(wheels.NumberOfWheels);

        if (vehicle is IHasSeats seats)
            Assert.Null(seats.NumberOfSeats);
    }

    [Theory]
    [InlineData("Car")]
    [InlineData("Motorcycle")]
    [InlineData("Bus")]
    [InlineData("Boat")]
    [InlineData("Airplane")]
    public void VehicleSpecification_AddFindAndRemoveEngine_UpdatesCollectionAndHandlesMissingEngine(string vehicleType)
    {
        // Arrange
        Vehicle vehicle = CreateMinimalVehicle(vehicleType);
        var specification = Assert.IsAssignableFrom<VehicleSpecification>(vehicle.SpecificationDetails);
        Engine engine = CreateEngine("ENG-ONE");

        // Act
        specification.AddEngine(engine);
        Engine? found = specification.FindEngine("eng-one");
        int index = specification.FindEngineIndex("eng-one");

        // Assert
        Assert.Same(engine, found);
        Assert.Equal(0, index);
        Assert.Single(specification.Engines);
        Assert.True(specification.RemoveEngine(engine));
        Assert.Empty(specification.Engines);
        Assert.Null(specification.FindEngine("ENG-ONE"));
        Assert.Equal(-1, specification.FindEngineIndex("ENG-ONE"));
        Assert.False(specification.RemoveEngine(engine));
    }

    [Theory]
    [InlineData("Car")]
    [InlineData("Motorcycle")]
    [InlineData("Bus")]
    [InlineData("Boat")]
    [InlineData("Airplane")]
    public void CreateVehicle_EngineListsModifiedExternally_DoesNotChangeStoredCollection(string vehicleType)
    {
        // Arrange
        Engine engine = CreateEngine("ENG-ORIGINAL");
        List<Engine> input = [engine];
        Vehicle vehicle = CreateMinimalVehicle(vehicleType, input);
        var specification = Assert.IsAssignableFrom<VehicleSpecification>(vehicle.SpecificationDetails);

        // Act
        input.Clear();
        List<Engine> returned = specification.Engines;
        returned.Clear();

        // Assert
        Assert.Same(engine, Assert.Single(specification.Engines));
    }

    [Theory]
    [InlineData("Car")]
    [InlineData("Motorcycle")]
    [InlineData("Bus")]
    [InlineData("Boat")]
    [InlineData("Airplane")]
    public void VehicleSpecification_EngineLimitReached_RejectsAnotherEngineWithoutChangingCollection(string vehicleType)
    {
        // Arrange
        var engines = Enumerable.Range(1, VehicleSpecification.MaximumNumberOfEngines)
            .Select(index => (Engine)CreateEngine($"ENG-{index}"))
            .ToList();

        Vehicle vehicle = CreateMinimalVehicle(vehicleType, engines);
        var specification = Assert.IsAssignableFrom<VehicleSpecification>(vehicle.SpecificationDetails);
        Engine extra = CreateEngine("ENG-EXTRA");

        // Act
        Action act = () => specification.AddEngine(extra);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
        Assert.Equal(VehicleSpecification.MaximumNumberOfEngines, specification.Engines.Count);
        Assert.Null(specification.FindEngine("ENG-EXTRA"));
    }

    [Fact]
    public void Vehicle_EngineSpecification_IsAccessibleThroughStoredEngine()
    {
        // Arrange
        Engine engine = CreateEngine("ENG-DATA", FuelType.Diesel);
        Motorcycle vehicle = MotorcycleFactory.CreateMotorcycle("MCY123", CountryCode.SWE, engines: [engine]);

        // Act
        var vehicleSpecification = Assert.IsType<MotorcycleSpecification>(vehicle.SpecificationDetails);
        Engine stored = Assert.Single(vehicleSpecification.Engines);
        var engineSpecification = Assert.IsType<PistonEngineSpecification>(stored.SpecificationDetails);

        // Assert
        Assert.Equal("ENG-DATA", stored.SerialNumber);
        Assert.Equal("Test inline engine", stored.Model);
        Assert.Equal(FuelType.Diesel, stored.FuelType);
        Assert.Equal(150, engineSpecification.HorsePower);
        Assert.Equal(2000, engineSpecification.DisplacementCc);
        Assert.Equal(4, engineSpecification.CylinderCount);
        Assert.Equal(500d, stored.CylinderVolumeCc);
    }

    [Fact]
    public void CreateMotorcycle_SidecarExplicitlyFalse_PreservesFalseInsteadOfUnknown()
    {
        // Arrange / Act
        Motorcycle vehicle = MotorcycleFactory.CreateMotorcycle("MCY123", CountryCode.SWE, hasSidecar: false);

        // Assert
        var specification = Assert.IsType<MotorcycleSpecification>(vehicle.Specification);
        Assert.Equal(false, specification.HasSidecar);
    }

    [Fact]
    public void CreateCar_ZeroCrewSeats_PreservesZeroAndCalculatesPassengerOnlyTotal()
    {
        // Arrange / Act
        Car vehicle = CarFactory.CreateCar("ABC123", CountryCode.SWE, numberOfSeats: 4, crewSeats: 0);

        // Assert
        var specification = Assert.IsType<CarSpecification>(vehicle.Specification);
        var cabin = Assert.IsType<Cabin>(specification.Cabin);
        Assert.Equal(0, cabin.CrewSeatCount);
        Assert.Equal(0, vehicle.DriverSeats);
        Assert.Equal(4, vehicle.PassengerCapacity);
        Assert.Equal(4, vehicle.NumberOfSeats);
    }

    [Fact]
    public void CreateBoat_CabinExplicitlyAbsent_DoesNotConstructACabin()
    {
        // Arrange / Act
        Boat vehicle = BoatFactory.CreateBoat("SABC", CountryCode.SWE, hasCabin: false);

        // Assert
        var specification = Assert.IsType<BoatSpecification>(vehicle.Specification);
        Assert.Null(specification.Cabin);
        Assert.Equal(false, vehicle.HasCabin);
    }

    [Fact]
    public void CreateAirplane_DualControlsExplicitlyFalse_PreservesFalseAndSinglePilotSeat()
    {
        // Arrange / Act
        Airplane vehicle = AirplaneFactory.CreateAirplane("SE-ABC", CountryCode.SWE, pilotSeatCount: 1, hasDualControls: false);

        // Assert
        var specification = Assert.IsType<AirplaneSpecification>(vehicle.Specification);
        var cockpit = Assert.IsType<Cockpit>(specification.Cockpit);
        Assert.Equal(false, cockpit.HasDualControls);
        Assert.Equal(1, cockpit.PilotSeats);
        Assert.Equal(1, vehicle.DriverSeats);
    }

    [Theory]
    [InlineData("Car")]
    [InlineData("Motorcycle")]
    [InlineData("Bus")]
    [InlineData("Boat")]
    [InlineData("Airplane")]
    public void CreateVehicle_OnlyOneDimensionProvided_RejectsIncompleteDimensions(string vehicleType)
    {
        // Arrange
        double length = 2;

        // Act
        Action act = () => CreateMinimalVehicle(vehicleType, length: length);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    private static void AssertCommon(Vehicle vehicle, string registration, string manufacturer, string model, int year, VehicleColor color, double weight, Dimensions dimensions, params Engine[] engines)
    {
        Assert.Equal(registration, vehicle.RegistrationNumber);
        Assert.Equal(CountryCode.SWE, vehicle.CountryCode);
        Assert.Equal(manufacturer, vehicle.Manufacturer);
        Assert.Equal(model, vehicle.Model);
        Assert.Equal(year, vehicle.ManufacturingYear);
        Assert.Equal(color, vehicle.Color);

        var specification = Assert.IsAssignableFrom<VehicleSpecification>(vehicle.SpecificationDetails);
        Assert.Equal(weight, specification.WeightKg);
        Assert.Equal(dimensions, specification.Dimensions);

        var actualEngines = specification.Engines;
        Assert.Equal(engines.Length, actualEngines.Count);

        for (int index = 0; index < engines.Length; index++)
            Assert.Same(engines[index], actualEngines[index]);
    }

    private static Vehicle CreateMinimalVehicle(string vehicleType, List<Engine>? engines = null, double? length = null)
    {
        return vehicleType switch
        {
            "Car" => CarFactory.CreateCar("ABC123", CountryCode.SWE, engines: engines, length: length),
            "Motorcycle" => MotorcycleFactory.CreateMotorcycle("MCY123", CountryCode.SWE, engines: engines, lengthMeters: length),
            "Bus" => BusFactory.CreateBus("BUS123", CountryCode.SWE, engines: engines, lengthMeters: length),
            "Boat" => BoatFactory.CreateBoat("SABC", CountryCode.SWE, engines: engines, lengthMeters: length),
            "Airplane" => AirplaneFactory.CreateAirplane("SE-ABC", CountryCode.SWE, engines: engines, lengthMeters: length),
            _ => throw new ArgumentException("Unsupported test vehicle type.", nameof(vehicleType))
        };
    }

    private static InlineEngine CreateEngine(string serialNumber, FuelType fuelType = FuelType.Gasoline)
    {
        return new InlineEngine
        {
            SerialNumber = serialNumber,
            Model = "Test inline engine",
            Specification = new PistonEngineSpecification
            {
                FuelType = fuelType,
                HorsePower = 150,
                DisplacementCc = 2000,
                CylinderCount = 4
            }
        };
    }

    private static FlatEngine CreateAircraftEngine(string serialNumber)
    {
        return new FlatEngine
        {
            SerialNumber = serialNumber,
            Model = "Test aircraft engine",
            Specification = new PistonEngineSpecification
            {
                FuelType = FuelType.AviationGasoline,
                HorsePower = 180,
                DisplacementCc = 6000,
                CylinderCount = 6
            }
        };
    }
}
