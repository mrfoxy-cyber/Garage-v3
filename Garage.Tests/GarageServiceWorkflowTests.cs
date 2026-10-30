using Garage.Models;
using Garage.Results;
using Garage.Services;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class GarageServiceWorkflowTests
{
    [Fact]
    public void CreateAndPark_RegistersTheSameVehicleInCatalogAndGarage()
    {
        VehicleGarage garage = new("Test", 2);
        GarageService service = new([garage], []);

        VehicleCreationResult result = service.VehicleCreation.Create(() => VehicleFixtures.CreateCar(), garage, 2);

        Assert.True(result.Success);
        Assert.Equal(OperationError.None, result.Error);
        Assert.Equal(2, result.Parking!.SpotNumber);
        Assert.Same(result.Vehicle, Assert.Single(service.Catalog.Vehicles));
        Assert.Same(result.Vehicle, Assert.Single(garage.ParkedVehicles).Vehicle);
    }

    [Fact]
    public void CreateAndPark_VehicleThatDoesNotFitDoesNotChangeEitherCollection()
    {
        VehicleGarage garage = new("Small", [new Dimensions(1, 1, 1)]);
        GarageService service = new([garage], []);

        VehicleCreationResult result = service.VehicleCreation.Create(() => VehicleFixtures.CreateCar(), garage, 1);

        Assert.False(result.Success);
        Assert.Equal(OperationError.VehicleDoesNotFit, result.Error);
        Assert.Null(result.Vehicle);
        Assert.Empty(service.Catalog.Vehicles);
        Assert.Empty(garage.Vehicles);
    }

    [Fact]
    public void CreateAndPark_OccupiedSpotKeepsExistingVehicleAndCatalog()
    {
        Vehicle existing = VehicleFixtures.CreateCar("ONE123");
        VehicleGarage garage = new("Test", 1, [existing]);
        GarageService service = new([garage], []);

        VehicleCreationResult result = service.VehicleCreation.Create(
            () => VehicleFixtures.CreateCar("TWO123"), garage, 1);

        Assert.Equal(OperationError.SpotOccupied, result.Error);
        Assert.Same(existing, Assert.Single(service.Catalog.Vehicles));
        Assert.Same(existing, Assert.Single(garage.Vehicles));
    }

    [Fact]
    public void CreateVehicle_RejectsRegistrationAlreadyInAnotherGarage()
    {
        Vehicle existing = VehicleFixtures.CreateCar("ABC123");
        VehicleGarage garage = new("Test", 2, [existing]);
        GarageService service = new([garage], []);

        VehicleCreationResult result = service.VehicleCreation.Create(() => VehicleFixtures.CreateCar("abc123"));

        Assert.Equal(OperationError.DuplicateRegistration, result.Error);
        Assert.Same(existing, Assert.Single(service.Catalog.Vehicles));
    }

    [Fact]
    public void Park_VehicleMustBeRemovedBeforeParkingInAnotherGarage()
    {
        Vehicle vehicle = VehicleFixtures.CreateCar();
        VehicleGarage first = new("First", 1, [vehicle]);
        VehicleGarage second = new("Second", 1);
        GarageService service = new([first, second], []);

        Assert.Equal(OperationError.VehicleAlreadyParked, service.Park(vehicle, second).Error);
        Assert.Empty(second.Vehicles);
        Assert.True(first.Remove(vehicle.RegistrationNumber).Success);
        Assert.Contains(vehicle, service.GetParkableVehicles(second));
        Assert.True(service.Park(vehicle, second).Success);
        Assert.Single(service.Catalog.Vehicles);
    }

    [Fact]
    public void CreateAndPark_RejectsGarageOutsideTheService()
    {
        GarageService service = new([], []);
        VehicleGarage external = new("External", 1);

        VehicleCreationResult result = service.VehicleCreation.Create(() => VehicleFixtures.CreateCar(), external);

        Assert.False(result.Success);
        Assert.Empty(service.Catalog.Vehicles);
        Assert.Empty(external.Vehicles);
    }

    [Fact]
    public void Park_RejectsVehicleOutsideTheCatalog()
    {
        VehicleGarage garage = new("Test", 1);
        GarageService service = new([garage], []);

        Assert.Equal(OperationError.VehicleNotFound, service.Park(VehicleFixtures.CreateCar(), garage).Error);
        Assert.Empty(garage.Vehicles);
    }

    [Fact]
    public void CatalogAndGarageCollections_CannotBeMutatedByCallers()
    {
        Vehicle vehicle = VehicleFixtures.CreateCar();
        VehicleGarage garage = new("Test", 2, [vehicle]);
        GarageService service = new([garage], []);

        Assert.Throws<NotSupportedException>(() => ((IList<Vehicle>)service.Catalog.Vehicles).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<Vehicle>)garage.Vehicles).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<VehicleGarage>)service.Garages).Clear());
        Assert.Single(garage.Vehicles);
    }

    [Fact]
    public void Catalog_RejectsDuplicateRegistrationInInitialData()
    {
        Assert.Throws<ArgumentException>(() => new VehicleCatalog(
            [VehicleFixtures.CreateCar("ABC123"), VehicleFixtures.CreateCar("abc123")]));
    }

    [Fact]
    public void FromJson_InvalidFileReturnsErrorWithoutChangingState()
    {
        GarageService service = new([], []);
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{ invalid json }");
            VehicleCreationResult result = service.VehicleCreation.FromJson(path);
            Assert.Equal(OperationError.InvalidJson, result.Error);
            Assert.Empty(service.Catalog.Vehicles);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void FromJson_MissingFileReturnsSpecificError()
    {
        GarageService service = new([], []);
        string missing = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        Assert.Equal(OperationError.FileNotFound, service.VehicleCreation.FromJson(missing).Error);
        Assert.Equal(OperationError.FileNotFound, service.CreateGarageFromJson(missing).Error);
    }

    [Fact]
    public void ResultFactories_RejectInconsistentSuccessAndFailure()
    {
        Assert.Throws<ArgumentNullException>(() => VehicleCreationResult.Completed(null!));
        Assert.Throws<ArgumentNullException>(() => GarageCreationResult.Completed(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => ParkingResult.Completed(0, "Invalid"));
        Assert.Throws<ArgumentException>(() => VehicleCreationResult.Failed("Invalid", OperationError.None));
        Assert.Throws<ArgumentException>(() => GarageCreationResult.Failed("Invalid", OperationError.None));
        Assert.Throws<ArgumentException>(() => ParkingResult.Failed("Invalid", OperationError.None));
        Assert.Throws<ArgumentException>(() => VehicleCreationResult.Completed(VehicleFixtures.CreateCar(),
            ParkingResult.Failed("Full", OperationError.GarageFull)));
    }
}
