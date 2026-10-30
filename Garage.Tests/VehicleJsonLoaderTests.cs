using Garage.Persistence.Json;

namespace Garage.Tests;

public class VehicleJsonLoaderTests
{
    [Fact]
    public void LoadVehicles_RejectsNullJsonRoot()
    {
        string path = Path.GetTempFileName();

        try
        {
            File.WriteAllText(path, "null");

            Exception exception = Assert.Throws(
                typeof(InvalidDataException),
                () => VehicleJsonLoader.LoadVehicles(path));

            Assert.Contains("JSON array", exception.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadVehicles_RejectsNullVehicle()
    {
        string path = Path.GetTempFileName();

        try
        {
            File.WriteAllText(path, "[null]");

            Exception exception = Assert.Throws(
                typeof(InvalidDataException),
                () => VehicleJsonLoader.LoadVehicles(path));

            Assert.Contains("cannot be null", exception.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
