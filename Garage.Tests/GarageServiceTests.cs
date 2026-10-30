using Garage.Services;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class GarageServiceTests
{
    [Fact]
    public void AddGarage_RejectsDuplicateNameWithoutChangingCollection()
    {
        GarageService system = new([new VehicleGarage("Lexicon", 2)], []);

        Assert.False(system.CreateGarage("LEXICON", 3).Success);

        Assert.Single(system.Garages);
        Assert.Equal(2, system.Garages[0].Capacity);
    }

    [Fact]
    public void Constructor_RejectsDuplicateInitialGarages()
    {
        Assert.Throws<ArgumentException>(() => new GarageService(
            [new VehicleGarage("Lexicon", 2), new VehicleGarage("lexicon", 3)], []));
    }

    [Fact]
    public void ImportGarage_FailedLoadDoesNotRegisterGarage()
    {
        GarageService system = new([], []);
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        Assert.False(system.CreateGarageFromJson(path).Success);

        Assert.Empty(system.Garages);
    }
}
