using Garage.Manager;
using Xunit;

namespace Garage.Tests;

public class GarageJsonLoaderTests
{
    [Fact]
    public void LoadGarage_ValidFile_RegistersGarageWithAllVehicles()
    {
        // Arrange
        string path = Path.Combine(
            AppContext.BaseDirectory, "Data", "example-garage.json");

        GarageManager manager = new();

        // Act
        var (garageId, loadResult) = manager.LoadGarage(path);

        // Assert
        Assert.True(loadResult.Success, loadResult.Message);
        Assert.NotNull(garageId);

        var (info, infoResult) = manager.GetGarageInfo(garageId);

        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);

        Assert.Equal("Test garage", info.GarageName);
        Assert.Equal(5, info.Capacity);
        Assert.Equal(5, info.OccupiedSpaces);

        var (vehicles, vehiclesResult) = manager.GetVehicles(garageId);

        Assert.True(vehiclesResult.Success, vehiclesResult.Message);

        Assert.Equal(
            new[] { "Airplane", "Boat", "Bus", "Car", "Motorcycle" },
            vehicles.Select(entry => entry.Vehicle.GetType().Name).OrderBy(name => name));
    }
}