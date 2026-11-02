using Garage.Factories;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using Garage.Vehicles.Specifications;
using Garage.Vehicles.Types;
using Garage_V3.Interfaces;
using Garage_V3.queries;
using Garage_V3.results;
using Garage_V3.Services;
using Vehicles.Interfaces;
using Xunit;

namespace Garage.Tests;

public sealed class GarageSearchTests
{
    [Flags]
    public enum Filters
    {
        None = 0,
        Registration = 1,
        VehicleType = 2,
        Country = 4,
        Manufacturer = 8,
        Model = 16,
        Year = 32,
        Color = 64,
        Wheels = 128
    }

    private sealed record Candidate(IVehicle Vehicle, Filters DifferentFromTarget);

    public static IEnumerable<object[]> FilterCombinations()
    {
        // Eight optional filters have 255 non-empty combinations, including each filter alone.
        for (int mask = 1; mask < 256; mask++)
            yield return [(Filters)mask];
    }

    [Theory]
    [MemberData(nameof(FilterCombinations))]
    public void SearchVehicles_SuppliedFilterCombination_ReturnsOnlyVehiclesMatchingEveryFilter(Filters filters)
    {
        // Arrange
        Candidate[] candidates = CreateCandidates();
        IHandler handler = CreateHandler(candidates.Select(candidate => candidate.Vehicle).ToArray());
        VehicleSearchCriteria criteria = CreateCriteria(filters);

        // The fixture table explicitly records which fields differ from the target.
        // Any candidate differing on a supplied field must be excluded.
        string[] expected = candidates
            .Where(candidate => (candidate.DifferentFromTarget & filters) == Filters.None)
            .Select(candidate => candidate.Vehicle.RegistrationNumber)
            .OrderBy(registration => registration, StringComparer.Ordinal)
            .ToArray();

        // Act
        var (matches, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Null(result.Error);
        Assert.Equal(expected, matches
            .Select(entry => entry.Vehicle.RegistrationNumber)
            .OrderBy(registration => registration, StringComparer.Ordinal));

        foreach (var match in matches)
        {
            Assert.Contains(candidates, candidate => ReferenceEquals(candidate.Vehicle, match.Vehicle));
            Assert.InRange(match.SpotNumber, 1, candidates.Length);
        }
    }

    [Fact]
    public void SearchVehicles_AllFiltersOmitted_ReturnsEveryVehicleIncludingUnknownDetails()
    {
        // Arrange
        Candidate[] candidates = CreateCandidates();
        IHandler handler = CreateHandler(candidates.Select(candidate => candidate.Vehicle).ToArray());

        // Act
        var (matches, result) = handler.SearchVehicles(new VehicleSearchCriteria());

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(candidates.Length, matches.Count);
        Assert.Equal(candidates.Select(candidate => candidate.Vehicle.RegistrationNumber), matches.Select(entry => entry.Vehicle.RegistrationNumber));
        Assert.Contains(matches, entry => entry.Vehicle.RegistrationNumber == "MIN001");
    }

    [Fact]
    public void SearchVehicles_WhitespaceTextFilters_TreatsThemAsOmitted()
    {
        // Arrange
        Candidate[] candidates = CreateCandidates();
        IHandler handler = CreateHandler(candidates.Select(candidate => candidate.Vehicle).ToArray());
        VehicleSearchCriteria criteria = new()
        {
            RegistrationNumber = " ",
            VehicleType = "\t",
            Manufacturer = "  ",
            Model = "\r\n"
        };

        // Act
        var (matches, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(candidates.Length, matches.Count);
    }

    [Theory]
    [InlineData("Registration")]
    [InlineData("VehicleType")]
    [InlineData("Manufacturer")]
    [InlineData("Model")]
    public void SearchVehicles_TextFilterHasDifferentCaseAndSurroundingWhitespace_MatchesIgnoringBoth(string field)
    {
        // Arrange
        Candidate[] candidates = CreateCandidates();
        IHandler handler = CreateHandler(candidates.Select(candidate => candidate.Vehicle).ToArray());

        var (criteria, expected) = field switch
        {
            "Registration" => (new VehicleSearchCriteria { RegistrationNumber = " mAt001 " }, new[] { "MAT001" }),
            "VehicleType" => (new VehicleSearchCriteria { VehicleType = " cAr " }, AllExcept("TYP001")),
            "Manufacturer" => (new VehicleSearchCriteria { Manufacturer = " vOlVo " }, AllExcept("MAN001", "MIN001")),
            "Model" => (new VehicleSearchCriteria { Model = " v70 " }, AllExcept("MOD001", "MIN001")),
            _ => throw new ArgumentException("Unknown test field.", nameof(field))
        };

        // Act
        var (matches, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(expected.OrderBy(value => value, StringComparer.Ordinal), matches
            .Select(entry => entry.Vehicle.RegistrationNumber)
            .OrderBy(value => value, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("Registration")]
    [InlineData("VehicleType")]
    [InlineData("Manufacturer")]
    [InlineData("Model")]
    public void SearchVehicles_OnlyPartOfTextProvided_DoesNotMatchExactTextFilters(string field)
    {
        // Arrange
        IHandler handler = CreateHandler(CreateCar("MAT001"));

        VehicleSearchCriteria criteria = field switch
        {
            "Registration" => new() { RegistrationNumber = "MAT" },
            "VehicleType" => new() { VehicleType = "Ca" },
            "Manufacturer" => new() { Manufacturer = "vol" },
            "Model" => new() { Model = "V7" },
            _ => throw new ArgumentException("Unknown test field.", nameof(field))
        };

        // Act
        var (matches, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(matches);
    }

    [Fact]
    public void SearchVehicles_NoMatchingVehicles_ReturnsSuccessfulEmptyCollection()
    {
        // Arrange
        IHandler handler = CreateHandler(CreateCar("MAT001"));
        VehicleSearchCriteria criteria = new() { Color = VehicleColor.Pink };

        // Act
        var (matches, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Null(result.Error);
        Assert.Empty(matches);
    }

    [Fact]
    public void SearchVehicles_EmptyGarage_ReturnsSuccessfulEmptyCollection()
    {
        // Arrange
        IHandler handler = CreateHandler();

        // Act
        var (matches, result) = handler.SearchVehicles(new VehicleSearchCriteria());

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(matches);
    }

    [Fact]
    public void SearchVehicles_NoGarageCreated_ReturnsNoGarageFailure()
    {
        // Arrange
        IHandler handler = new GarageHandler();

        // Act
        var (matches, result) = handler.SearchVehicles(new VehicleSearchCriteria());

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.NoGarage, result.Error);
        Assert.Empty(matches);
    }

    [Fact]
    public void SearchVehicles_NullCriteria_ReturnsValidationFailure()
    {
        // Arrange
        IHandler handler = CreateHandler(CreateCar("MAT001"));

        // Act
        var (matches, result) = handler.SearchVehicles(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(OperationError.ValidationFailed, result.Error);
        Assert.Empty(matches);
    }

    [Theory]
    [InlineData("Manufacturer")]
    [InlineData("Model")]
    [InlineData("Year")]
    [InlineData("Color")]
    [InlineData("Wheels")]
    public void SearchVehicles_RequestedValueIsUnknownOnVehicle_DoesNotMatch(string field)
    {
        // Arrange
        IHandler handler = CreateHandler(CarFactory.CreateCar("MIN001", CountryCode.SWE));

        VehicleSearchCriteria criteria = field switch
        {
            "Manufacturer" => new() { Manufacturer = "Volvo" },
            "Model" => new() { Model = "V70" },
            "Year" => new() { ManufacturingYear = 2020 },
            "Color" => new() { Color = VehicleColor.Black },
            "Wheels" => new() { NumberOfWheels = 4 },
            _ => throw new ArgumentException("Unknown test field.", nameof(field))
        };

        // Act
        var (matches, result) = handler.SearchVehicles(criteria);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Empty(matches);
    }

    [Fact]
    public void SearchVehicles_ZeroWheelsRequested_MatchesKnownZeroButNotUnknownOrUnsupported()
    {
        // Arrange
        Car zeroWheels = CreateCar("ZER001", wheels: 0);
        Car unknownWheels = CarFactory.CreateCar("MIN001", CountryCode.SWE);
        Boat noWheelInterface = BoatFactory.CreateBoat("SABC", CountryCode.SWE);
        IHandler handler = CreateHandler(zeroWheels, unknownWheels, noWheelInterface);

        // Act
        var (matches, result) = handler.SearchVehicles(new VehicleSearchCriteria { NumberOfWheels = 0 });

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Same(zeroWheels, Assert.Single(matches).Vehicle);
    }

    [Theory]
    [InlineData("Car")]
    [InlineData("Motorcycle")]
    [InlineData("Bus")]
    [InlineData("Boat")]
    [InlineData("Airplane")]
    [InlineData("Truck")]
    public void SearchVehicles_VehicleTypeRequested_ReturnsOnlyThatType(string vehicleType)
    {
        // Arrange
        IHandler handler = CreateHandler(CreateAllTypes());

        // Act
        var (matches, result) = handler.SearchVehicles(new VehicleSearchCriteria { VehicleType = vehicleType });

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(vehicleType, Assert.Single(matches).Vehicle.GetType().Name);
    }

    [Fact]
    public void GetVehicleTypeCounts_MixedGarage_ReturnsEachTypeAndItsCount()
    {
        // Arrange
        var vehicles = CreateAllTypes().Append(CarFactory.CreateCar("CAR002", CountryCode.SWE)).ToArray();
        IHandler handler = CreateHandler(vehicles);

        // Act
        var (counts, result) = handler.GetVehicleTypeCounts();

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(6, counts.Count);
        Assert.Equal(2, Assert.Single(counts, count => count.VehicleType == nameof(Car)).Count);

        foreach (string type in new[] { nameof(Motorcycle), nameof(Bus), nameof(Boat), nameof(Airplane), nameof(Truck) })
            Assert.Equal(1, Assert.Single(counts, count => count.VehicleType == type).Count);

        Assert.Equal(vehicles.Length, counts.Sum(count => count.Count));
    }

    [Fact]
    public void SearchVehicles_RepeatedSearches_DoNotChangeVehiclesSpotsOrOccupancy()
    {
        // Arrange
        Candidate[] candidates = CreateCandidates();
        IHandler handler = CreateHandler(candidates.Select(candidate => candidate.Vehicle).ToArray());
        var (before, beforeResult) = handler.GetVehicles();
        Assert.True(beforeResult.Success, beforeResult.Message);

        // Act
        Assert.True(handler.SearchVehicles(new() { Color = VehicleColor.Black }).Result.Success);
        Assert.True(handler.SearchVehicles(new() { Color = VehicleColor.Pink }).Result.Success);
        Assert.True(handler.SearchVehicles(new()).Result.Success);

        // Assert
        var (after, afterResult) = handler.GetVehicles();
        Assert.True(afterResult.Success, afterResult.Message);
        Assert.Equal(before.Select(entry => (entry.SpotNumber, entry.Vehicle.RegistrationNumber)), after.Select(entry => (entry.SpotNumber, entry.Vehicle.RegistrationNumber)));

        var (info, infoResult) = handler.GetGarageInfo();
        Assert.True(infoResult.Success, infoResult.Message);
        Assert.NotNull(info);
        Assert.Equal(candidates.Length, info.OccupiedSpaces);
    }

    private static VehicleSearchCriteria CreateCriteria(Filters filters)
    {
        return new VehicleSearchCriteria
        {
            RegistrationNumber = filters.HasFlag(Filters.Registration) ? "MAT001" : null,
            VehicleType = filters.HasFlag(Filters.VehicleType) ? nameof(Car) : null,
            CountryCode = filters.HasFlag(Filters.Country) ? CountryCode.SWE : null,
            Manufacturer = filters.HasFlag(Filters.Manufacturer) ? "Volvo" : null,
            Model = filters.HasFlag(Filters.Model) ? "V70" : null,
            ManufacturingYear = filters.HasFlag(Filters.Year) ? 2020 : null,
            Color = filters.HasFlag(Filters.Color) ? VehicleColor.Black : null,
            NumberOfWheels = filters.HasFlag(Filters.Wheels) ? 4 : null
        };
    }

    private static Candidate[] CreateCandidates()
    {
        return
        [
            new(CreateCar("MAT001"), Filters.None),
            new(CreateCar("REG002"), Filters.Registration),
            new(new Motorcycle
            {
                RegistrationNumber = "TYP001",
                CountryCode = CountryCode.SWE,
                Manufacturer = "Volvo",
                Model = "V70",
                ManufacturingYear = 2020,
                Color = VehicleColor.Black,
                Specification = new MotorcycleSpecification(numberOfWheels: 4)
                {
                    Dimensions = new Dimensions(2, 1, 1)
                }
            }, Filters.Registration | Filters.VehicleType),
            new(CreateCar("FIN123", country: CountryCode.FIN), Filters.Registration | Filters.Country),
            new(CreateCar("MAN001", manufacturer: "Saab"), Filters.Registration | Filters.Manufacturer),
            new(CreateCar("MOD001", model: "XC90"), Filters.Registration | Filters.Model),
            new(CreateCar("YEA001", year: 2019), Filters.Registration | Filters.Year),
            new(CreateCar("COL001", color: VehicleColor.Red), Filters.Registration | Filters.Color),
            new(CreateCar("WHL001", wheels: 3), Filters.Registration | Filters.Wheels),
            new(CarFactory.CreateCar("MIN001", CountryCode.SWE), Filters.Registration | Filters.Manufacturer | Filters.Model | Filters.Year | Filters.Color | Filters.Wheels)
        ];
    }

    private static Car CreateCar(string registration, CountryCode country = CountryCode.SWE, string manufacturer = "Volvo", string model = "V70", int year = 2020, VehicleColor color = VehicleColor.Black, int wheels = 4)
    {
        // Controlled search data: each near-match changes one field independently of factory defaults.
        return new Car
        {
            RegistrationNumber = registration,
            CountryCode = country,
            Manufacturer = manufacturer,
            Model = model,
            ManufacturingYear = year,
            Color = color,
            Specification = new CarSpecification(numberOfWheels: wheels)
            {
                Dimensions = new Dimensions(4, 2, 1.5)
            }
        };
    }

    private static IHandler CreateHandler(params IVehicle[] vehicles)
    {
        IHandler handler = new GarageHandler();
        var creation = handler.CreateGarage("Search test garage", Math.Max(1, vehicles.Length));
        Assert.True(creation.Success, creation.Message);

        foreach (IVehicle vehicle in vehicles)
        {
            var parking = handler.ParkVehicle(vehicle);
            Assert.True(parking.Success, parking.Message);
        }

        return handler;
    }

    private static IVehicle[] CreateAllTypes()
    {
        return
        [
            CarFactory.CreateCar("CAR001", CountryCode.SWE),
            MotorcycleFactory.CreateMotorcycle("MCY001", CountryCode.SWE),
            BusFactory.CreateBus("BUS001", CountryCode.SWE),
            BoatFactory.CreateBoat("SABC", CountryCode.SWE),
            AirplaneFactory.CreateAirplane("SE-ABC", CountryCode.SWE),
            TruckFactory.CreateTruck("TRK001", CountryCode.SWE)
        ];
    }

    private static string[] AllExcept(params string[] excluded)
    {
        string[] registrations = ["MAT001", "REG002", "TYP001", "FIN123", "MAN001", "MOD001", "YEA001", "COL001", "WHL001", "MIN001"];
        return registrations.Except(excluded, StringComparer.Ordinal).ToArray();
    }
}
