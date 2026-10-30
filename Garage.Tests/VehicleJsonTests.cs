using Garage.Persistence.Json;
using Garage.Vehicles;

namespace Garage.Tests;

public class VehicleJsonTests
{
    [Fact]
    public void LoadVehicle_CreatesCarAndEngineFromObject()
    {
        Vehicle vehicle = VehicleJsonLoader.LoadVehicle(Path.Combine(
            AppContext.BaseDirectory, "Data", "example-vehicle.json"));

        Assert.IsType<Car>(vehicle);
        Assert.Equal("NEW123", vehicle.RegistrationNumber);
        Assert.Equal("Volvo", vehicle.Manufacturer);
        Assert.Equal("ENG-VOLVO-001", Assert.Single(vehicle.SpecificationDetails.Engines).SerialNumber);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{invalid}")]
    [InlineData("{\"type\":\"Car\"}")]
    public void LoadVehicle_RejectsInvalidDocument(string json)
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, json);
            Assert.ThrowsAny<Exception>(() => VehicleJsonLoader.LoadVehicle(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
