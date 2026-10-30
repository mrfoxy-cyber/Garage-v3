using Garage.ConsoleApp.Drafts;
using Garage.Persistence.Json;
using Garage.Tests.Fixtures;
using Garage.Vehicles;

namespace Garage.Tests;

public class RegistrationValidationTests
{
    [Theory]
    [InlineData("ABC123")]
    [InlineData("ABC12D")]
    [InlineData(" abc-123 ")]
    public void RoadVehicle_NormalizesSupportedRegistrations(string number)
    {
        var car = VehicleFixtures.CreateCar(number);

        Assert.Equal(number.Trim().ToUpperInvariant().Replace("-", ""), car.RegistrationNumber);
        Assert.Equal(CountryCode.SWE, car.CountryCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("---")]
    [InlineData("MATCH1")]
    [InlineData("ABC1234")]
    public void RoadVehicle_RejectsInvalidRegistrations(string number)
    {
        Assert.ThrowsAny<ArgumentException>(() => VehicleFixtures.CreateCar(number));
    }

    [Fact]
    public void Factory_UsesTheExplicitCountry()
    {
        var car = VehicleFixtures.CreateCar("AB-123-CD", countryCode: CountryCode.FRA);

        Assert.Equal(CountryCode.FRA, car.CountryCode);
        Assert.Equal("AB123CD", car.RegistrationNumber);
        Assert.Throws<ArgumentException>(() =>
            VehicleFixtures.CreateCar("AB-123-CD", countryCode: CountryCode.SWE));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Initializer_ValidatesUsingBothPropertiesRegardlessOfOrder(bool countryFirst)
    {
        var car = CreateCar("AB-123-CD", CountryCode.FRA, countryFirst);

        Assert.Equal("AB123CD", car.RegistrationNumber);
        Assert.Equal(CountryCode.FRA, car.CountryCode);
        Assert.Throws<ArgumentException>(() => CreateCar("ABC123", CountryCode.FRA, countryFirst));
    }

    [Theory]
    [InlineData(CountryCode.Unspecified)]
    [InlineData((CountryCode)999)]
    public void Vehicle_RejectsMissingOrUndefinedCountry(CountryCode countryCode)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VehicleFixtures.CreateCar(countryCode: countryCode));
    }

    [Fact]
    public void Boat_UsesBoatRulesInsteadOfRoadRules()
    {
        var boat = VehicleFixtures.CreateBoat(registrationNumber: " sfb-2345 ");

        Assert.Equal("SFB-2345", boat.RegistrationNumber);
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

        Assert.Equal("D-ABCD", airplane.RegistrationNumber);
        Assert.Throws<ArgumentException>(() => VehicleFixtures.CreateAirplane(
            registrationNumber: "D-ABCD", countryCode: CountryCode.SWE));
    }

    [Fact]
    public void Draft_RequiresCountryAndClearsItAfterUse()
    {
        var draft = new CarDraft();
        Assert.Contains("country of registration", draft.GetMissingFields());

        draft.CountryCode = CountryCode.SWE;
        Assert.DoesNotContain("country of registration", draft.GetMissingFields());

        draft.Clear();
        Assert.Null(draft.CountryCode);
    }

    [Theory]
    [InlineData("vehicles.json", 30)]
    [InlineData("created-vehicles.json", 12)]
    public void SampleData_LoadsWithValidRegistrations(string fileName, int expectedCount)
    {
        var vehicles = VehicleJsonLoader.LoadVehicles(fileName);

        Assert.Equal(expectedCount, vehicles.Count);
        Assert.All(vehicles, vehicle => Assert.Equal(CountryCode.SWE, vehicle.CountryCode));
    }

    private static Car CreateCar(string number, CountryCode countryCode, bool countryFirst)
    {
        var template = VehicleFixtures.CreateCar();

        if (countryFirst)
        {
            return new Car
            {
                CountryCode = countryCode,
                RegistrationNumber = number,
                Manufacturer = template.Manufacturer,
                Model = template.Model,
                ManufacturingYear = template.ManufacturingYear,
                Color = template.Color,
                Vin = template.Vin,
                Specification = template.Specification
            };
        }

        return new Car
        {
            RegistrationNumber = number,
            CountryCode = countryCode,
            Manufacturer = template.Manufacturer,
            Model = template.Model,
            ManufacturingYear = template.ManufacturingYear,
            Color = template.Color,
            Vin = template.Vin,
            Specification = template.Specification
        };
    }
}
