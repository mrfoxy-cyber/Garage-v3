using Garage.Persistence.Json;

namespace Garage.Tests;

public class GarageJsonLoaderTests
{
    [Fact]
    public void Load_CreatesGarageWithCustomSpotSizeAndAssignments()
    {
        string directory = CreateDirectory();
        try
        {
            string configuration = Write(directory, "garage.json", """
                { "garageName": "Test", "capacity": 2,
                  "defaultSpotSize": { "lengthMeters": 3, "widthMeters": 2, "heightMeters": 2 },
                  "spots": [{ "spotNumber": 2,
                    "size": { "lengthMeters": 6, "widthMeters": 3, "heightMeters": 3 } }] }
                """);
            Write(directory, "vehicles.json", VehicleJson);
            string placements = Write(directory, "placements.json", """
                { "garageName": "Test", "vehiclesFile": "vehicles.json",
                  "assignments": [{ "spotNumber": 2, "registrationNumber": "abc123" }] }
                """);

            var garage = GarageJsonLoader.Load(configuration, placements);

            Assert.Equal(2, garage.Capacity);
            Assert.Equal(1, garage.OccupiedSpaces);
            Assert.Equal(2, Assert.Single(garage.ParkedVehicles).SpotNumber);
            Assert.Equal(6, garage.SpotDetails[1].Dimensions!.LengthMeters);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Fact]
    public void Load_RejectsDuplicateCustomSpotNumbers()
    {
        string directory = CreateDirectory();
        try
        {
            string path = Write(directory, "garage.json", """
                { "garageName": "Test", "capacity": 2,
                  "defaultSpotSize": { "lengthMeters": 5, "widthMeters": 3, "heightMeters": 3 },
                  "spots": [
                    { "spotNumber": 1, "size": { "lengthMeters": 6, "widthMeters": 3, "heightMeters": 3 } },
                    { "spotNumber": 1, "size": { "lengthMeters": 7, "widthMeters": 3, "heightMeters": 3 } }
                  ] }
                """);

            Assert.Throws<InvalidDataException>(() => GarageJsonLoader.Load(path));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Fact]
    public void Load_RejectsVehicleThatDoesNotFitItsAssignedSpot()
    {
        string directory = CreateDirectory();
        try
        {
            string configuration = Write(directory, "garage.json", """
                { "garageName": "Test", "capacity": 1,
                  "defaultSpotSize": { "lengthMeters": 2, "widthMeters": 2, "heightMeters": 2 } }
                """);
            Write(directory, "vehicles.json", VehicleJson);
            string placements = Write(directory, "placements.json", """
                { "garageName": "Test", "vehiclesFile": "vehicles.json",
                  "assignments": [{ "spotNumber": 1, "registrationNumber": "ABC123" }] }
                """);

            Assert.Throws<InvalidDataException>(() => GarageJsonLoader.Load(configuration, placements));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    private static string CreateDirectory()
    {
        string path = Path.Combine(Path.GetTempPath(), $"garage-spec-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private static string Write(string directory, string name, string content)
    {
        string path = Path.Combine(directory, name);
        File.WriteAllText(path, content);
        return path;
    }

    private const string VehicleJson = """
        [{ "type": "Car", "registrationNumber": "ABC123", "countryCode": "SWE",
           "manufacturer": "Volvo", "model": "V70", "manufacturingYear": 2018,
           "color": "Black", "weightKg": 1700, "lengthMeters": 4.8,
           "widthMeters": 1.8, "heightMeters": 1.5, "vin": "VOLVO-V70-001",
           "bodyType": "Sedan", "numberOfDoors": 5, "numberOfSeats": 5,
           "engines": [{ "type": "Inline", "serialNumber": "ENG-VOLVO-001",
                         "model": "B4204T", "fuelType": "Gasoline",
                         "horsePower": 190, "displacementCc": 1969,
                         "cylinderCount": 4 }] }]
        """;
}
