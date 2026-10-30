using Garage.ConsoleApp.Display;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Specifications;

namespace Garage.Tests;

[CollectionDefinition("Console output", DisableParallelization = true)]
public class ConsoleOutputCollection
{
}

[Collection("Console output")]
public class VehicleEngineTests
{
    private static List<Vehicle> CreateVehicles() =>
    [
        VehicleFixtures.CreateCar(),
        VehicleFixtures.CreateMotorcycle(),
        VehicleFixtures.CreateBus(),
        VehicleFixtures.CreateAirplane(),
        VehicleFixtures.CreateBoat(),
        VehicleFixtures.CreateTruck()
    ];

    [Fact]
    public void AddEngine_PreservesExistingEngineAndAddsNewEngine()
    {
        foreach (Vehicle vehicle in CreateVehicles())
        {
            var original = Assert.Single(vehicle.SpecificationDetails.Engines);
            var added = EngineFixtures.CreateEngine(serialNumber: "ADDED123");

            vehicle.SpecificationDetails.AddEngine(added);

            Assert.Equal(2, vehicle.SpecificationDetails.Engines.Count);
            Assert.Contains(original, vehicle.SpecificationDetails.Engines);
            Assert.Contains(added, vehicle.SpecificationDetails.Engines);
            Assert.Same(added, vehicle.SpecificationDetails.FindEngine("ADDED123"));
        }
    }

    [Fact]
    public void RemoveEngine_RemovesOnlySelectedEngine()
    {
        foreach (Vehicle vehicle in CreateVehicles())
        {
            var original = Assert.Single(vehicle.SpecificationDetails.Engines);
            var added = EngineFixtures.CreateEngine(serialNumber: "KEEP123");
            vehicle.SpecificationDetails.AddEngine(added);

            bool removed = vehicle.SpecificationDetails.RemoveEngine(original);

            Assert.True(removed);
            Assert.Same(added, Assert.Single(vehicle.SpecificationDetails.Engines));
            Assert.Null(vehicle.SpecificationDetails.FindEngine(original.SerialNumber));
            Assert.False(vehicle.SpecificationDetails.RemoveEngine(original));
            Assert.Same(added, Assert.Single(vehicle.SpecificationDetails.Engines));
        }
    }

    [Fact]
    public void AddEngine_RejectsEngineBeyondCapacity()
    {
        var car = VehicleFixtures.CreateCar();
        for (int i = 1; i < VehicleSpecification.MaximumNumberOfEngines; i++)
        {
            car.Specification.AddEngine(
                EngineFixtures.CreateEngine(serialNumber: $"ENG-{i}"));
        }
        var extra = EngineFixtures.CreateEngine(serialNumber: "EXTRA");

        Assert.Throws<InvalidOperationException>(() => car.Specification.AddEngine(extra));

        Assert.Equal(VehicleSpecification.MaximumNumberOfEngines, car.Specification.Engines.Count);
        Assert.DoesNotContain(extra, car.Specification.Engines);
    }

    [Fact]
    public void PrintVehicle_ReflectsAddedAndRemovedEngines()
    {
        foreach (Vehicle vehicle in CreateVehicles())
        {
            var original = Assert.Single(vehicle.SpecificationDetails.Engines);
            var added = EngineFixtures.CreateEngine(
                serialNumber: "PRINT123", model: "Test engine", horsePower: 240);
            vehicle.SpecificationDetails.AddEngine(added);

            string beforeRemoval = CaptureVehicle(vehicle);

            Assert.Contains("Spot: 3", beforeRemoval);
            Assert.Contains($"Registration number: {vehicle.RegistrationNumber}", beforeRemoval);
            Assert.Contains($"Serial number: {original.SerialNumber}", beforeRemoval);
            Assert.Contains("Serial number: PRINT123", beforeRemoval);
            Assert.Contains("Model: Test engine", beforeRemoval);
            Assert.Contains("Fuel type: Gasoline", beforeRemoval);
            Assert.Contains("Horsepower: 240", beforeRemoval);
            Assert.Contains("Total displacement: 2000 cc", beforeRemoval);
            Assert.Contains("Cylinder count: 4", beforeRemoval);
            Assert.DoesNotContain("No engines.", beforeRemoval);

            Assert.True(vehicle.SpecificationDetails.RemoveEngine(added));
            string afterRemoval = CaptureVehicle(vehicle);
            Assert.DoesNotContain("Serial number: PRINT123", afterRemoval);
            Assert.Contains($"Serial number: {original.SerialNumber}", afterRemoval);

            Assert.True(vehicle.SpecificationDetails.RemoveEngine(original));
            string empty = CaptureVehicle(vehicle);
            Assert.Contains("No engines.", empty);
            Assert.DoesNotContain("Serial number:", empty);
        }
    }

    private static string CaptureVehicle(Vehicle vehicle)
    {
        TextWriter previous = Console.Out;
        using var output = new StringWriter();
        try
        {
            Console.SetOut(output);
            VehicleDisplay.PrintVehicle(3, vehicle);
            return output.ToString();
        }
        finally
        {
            Console.SetOut(previous);
        }
    }
}
