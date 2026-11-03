using Garage.Factories;
using Garage.Manager;
using Garage.Manager.Persistence.Json;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage_V3.results;
using Garage.Manager.Persistence;

namespace Garage.Tests;

public class RegistrationValidationTests
{
    [Theory]
    [InlineData("ABC123", CountryCode.SWE)]
    [InlineData("ABC12D", CountryCode.SWE)]
    [InlineData(" abc-123 ", CountryCode.SWE)]
    public void RoadVehicle_NormalizesSupportedRegistrations(string number, CountryCode countryCode)
    {
        var car = CarFactory.CreateCar(number, countryCode);

        Assert.Equal(number.Trim().Replace("-", ""), car.RegistrationNumber);
        Assert.Equal(countryCode, car.CountryCode);
    }

    [Theory]
    [InlineData("", CountryCode.SWE)]
    [InlineData(" ", CountryCode.SWE)]
    [InlineData("---", CountryCode.SWE)]
    [InlineData("MATCH1", CountryCode.SWE)]
    [InlineData("ABC1234", CountryCode.SWE)]
    public void RoadVehicle_RejectsInvalidRegistrations(string number, CountryCode countryCode)
    {
        Assert.ThrowsAny<ArgumentException>(() => CarFactory.CreateCar(number, countryCode));
    }

    [Fact]
    public void Factory_UsesTheExplicitCountry()
    {
        var car = CarFactory.CreateCar("AB-123-CD", countryCode: CountryCode.FRA);

        Assert.Equal(CountryCode.FRA, car.CountryCode);
        Assert.Equal("AB123CD", car.RegistrationNumber);
        Assert.Throws<ArgumentException>(() =>
            CarFactory.CreateCar("AB-123-CD", countryCode: CountryCode.SWE));
    }

    [Theory]
    [InlineData(CountryCode.Unspecified)]
    [InlineData((CountryCode)999)]
    public void Vehicle_RejectsMissingOrUndefinedCountry(CountryCode countryCode)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CarFactory.CreateCar("AB-123-CD", countryCode: countryCode));
    }

    [Fact]
    public void Boat_UsesBoatRulesInsteadOfRoadRules()
    {
        var boat = VehicleFixtures.CreateBoat(registrationNumber: " sfb-2345 ");

        Assert.Equal("sfb-2345", boat.RegistrationNumber);
        Assert.Throws<ArgumentException>(() =>
            VehicleFixtures.CreateBoat(registrationNumber: "ABC123"));
        Assert.Throws<NotSupportedException>(() =>
            VehicleFixtures.CreateBoat(countryCode: CountryCode.DEU));
    }

    [Fact]
    public void Airplane_UsesItsCountryPrefix()
    {
        var airplane = VehicleFixtures.CreateAirplane(
            registrationNumber: " d-abcd ", countryCode: CountryCode.DEU);

        Assert.Equal("d-abcd", airplane.RegistrationNumber);
        Assert.Throws<ArgumentException>(() => VehicleFixtures.CreateAirplane(
            registrationNumber: "D-ABCD", countryCode: CountryCode.SWE));
    }


    [Theory]
    [InlineData("example-garage.json", 5)]
    public void LoadGarage_ValidData_LoadsExpectedVehicles(string fileName, int expectedCount)
    {
        // Arrange
        GarageManager manager = new();
        string path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        Assert.True(File.Exists(path), $"Missing test data: {path}");

        // Act
        var (garageId, loadResult) = manager.LoadGarage(path);

        // Assert
        Assert.True(loadResult.Success, loadResult.Message);
        Assert.NotNull(garageId);

        var (vehicles, vehiclesResult) = manager.GetVehicles(garageId);
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Equal(expectedCount, vehicles.Count);
        Assert.All(vehicles, entry => Assert.Equal(CountryCode.SWE, entry.Vehicle.CountryCode));

        var (info, infoResult) = manager.GetGarageInfo(garageId);
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal(expectedCount, info.OccupiedSpaces);
    }

    [Theory]
    [InlineData("invalid-registration-garage.json", OperationError.ValidationFailed)]
    [InlineData("invalid-wheel-count-garage.json", OperationError.ValidationFailed)]
    [InlineData("duplicate-registrations-garage.json", OperationError.DuplicateRegistration)]
    [InlineData("over-capacity-garage.json", OperationError.GarageFull)]
    public void LoadGarage_InvalidData_ReturnsExpectedFailureAndLeavesExistingGarageUnchanged(string fileName, OperationError expectedError)
    {
        // Arrange
        GarageManager manager = new();
        var (existingId, creationResult) = manager.CreateGarage("Existing garage", 2);
        Assert.True(creationResult.Success, creationResult.Message);
        Assert.NotNull(existingId);

        var existingVehicle = CarFactory.CreateCar("OLD123", CountryCode.SWE);
        var parkingResult = manager.ParkVehicle(existingId, existingVehicle);
        Assert.True(parkingResult.Success, parkingResult.Message);

        string path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        Assert.True(File.Exists(path), $"Missing test data: {path}");

        // The file must parse successfully: this test targets model/garage validation, not malformed JSON.
        GarageFileData data = GarageJsonLoader.Read(path);
        Assert.Equal(2, data.Vehicles.Count);

        // Act
        var (loadedId, loadResult) = manager.LoadGarage(path);

        // Assert
        Assert.False(loadResult.Success);
        Assert.Equal(expectedError, loadResult.Error);
        Assert.Null(loadedId);
        Assert.False(string.IsNullOrWhiteSpace(loadResult.Message));

        var (garages, garagesResult) = manager.GetGarages();
        Assert.True(garagesResult.Success, garagesResult.Message);
        Assert.Equal(existingId, Assert.Single(garages).Id);

        var (vehicles, vehiclesResult) = manager.GetVehicles(existingId);
        Assert.True(vehiclesResult.Success, vehiclesResult.Message);
        Assert.Same(existingVehicle, Assert.Single(vehicles).Vehicle);

        // ABC123 was the first valid entry in every failed file. It must not remain reserved.
        var retry = manager.ParkVehicle(existingId, CarFactory.CreateCar("ABC123", CountryCode.SWE));
        Assert.True(retry.Success, retry.Message);
    }

}
