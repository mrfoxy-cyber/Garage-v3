using Garage.Factories;
using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Garage.Vehicles;
using Garage.Vehicles.Types;
using Garage_V3.queries;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Xunit;

namespace Garage.Tests.Gui;

[CollectionDefinition("Garage GUI", DisableParallelization = true)]
public sealed class GarageGuiCollection
{
}

[Collection("Garage GUI")]
public class MainWindowTests
{
    [Fact]
    public void CreateGarage_ValidInput_RaisesCreateRequest()
    {
        // Arrange
        using CreateGarageInput input = new(CreateTheme());
        (string Name, int Capacity)? request = null;
        int calls = 0;
        input.CreateRequested += (name, capacity) =>
        {
            request = (name, capacity);
            calls++;
        };
        input.Name.Text = "  My garage  ";
        input.Capacity.Text = "10";

        // Act
        PressButton(input, "Create garage");

        // Assert
        Assert.Equal(1, calls);
        Assert.True(request.HasValue);
        Assert.Equal("My garage", request.Value.Name);
        Assert.Equal(10, request.Value.Capacity);
        Assert.Empty(input.ErrorMessage);
    }

    [Fact]
    public void ParkVehicle_CarInput_RaisesParkingRequest()
    {
        // Arrange
        using ParkVehicleInput input = new(CreateTheme());
        ParkVehicleRequest? request = null;
        int calls = 0;
        input.ParkRequested += value =>
        {
            request = value;
            calls++;
        };
        input.SetAllowedTypes(["Car"]);

        // Find fields by their captions so this also supports nested input components.
        FindTextInput(input, "Registration number").Text = "  ABC123  ";
        FindTextInput(input, "Spot (blank = automatic)").Text = "2";

        // Act
        PressButton(input, "Park vehicle");

        // Assert
        Assert.Equal(1, calls);
        Assert.NotNull(request);
        Assert.Equal("Car", request.VehicleType);
        Assert.Equal("ABC123", request.RegistrationNumber);
        Assert.Equal(CountryCode.SWE, request.CountryCode);
        Assert.Equal(2, request.SpotNumber);
    }

    [Fact]
    public void RemoveVehicle_FoundCar_RequiresConfirmationBeforeRemovalRequest()
    {
        // Arrange
        GarageTheme theme = CreateTheme();
        using RemoveVehicleInput input = new(theme);
        using RemoveVehicleConfirmation confirmation = new(theme);
        string? lookup = null;
        LocatedVehicleInfo? removal = null;
        input.LookupRequested += registration => lookup = registration;
        confirmation.ConfirmRequested += match => removal = match;
        var car = CarFactory.CreateCar("ABC123", CountryCode.SWE);
        LocatedVehicleInfo expected = new("garage-1", "My garage", 3, car);
        input.Registration.Text = "  ABC123  ";

        // Act — look up the vehicle, then present the match.
        PressButton(input, "Find vehicle to remove");
        Assert.Equal("ABC123", lookup);
        Assert.Null(removal);
        confirmation.ShowMatch(expected);
        Assert.Null(removal);

        // Act — confirm removal.
        PressButton(confirmation, "Remove this vehicle");

        // Assert
        Assert.Same(expected, removal);
        Assert.Equal("garage-1", removal!.GarageId);
        Assert.Equal(3, removal.SpotNumber);
        Assert.Same(car, removal.Vehicle);
    }

    [Fact]
    public void VehicleTypeCounts_AllTypes_DisplaysEachTypeAndCount()
    {
        // Arrange
        using VehicleTypeCounts display = new(CreateTheme());
        VehicleTypeCount[] expected =
        [
            new("Car", 1), new("Motorcycle", 2), new("Bus", 3),
            new("Airplane", 4), new("Boat", 5), new("Truck", 6)
        ];

        // Act
        display.ShowCounts(expected);

        // Assert
        string[] lines = display.DisplayText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Equal(expected.Length, lines.Length);
        foreach (var item in expected)
        {
            string line = Assert.Single(lines, text => text.StartsWith(item.VehicleType + " ", StringComparison.Ordinal));
            string[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(item.VehicleType, values[0]);
            Assert.Equal(item.Count.ToString(), values[1]);
        }
    }

    [Fact]
    public void SearchVehicle_CombinedInput_RaisesCorrectSearchCriteria()
    {
        // Arrange
        using VehicleSearchInput input = new(CreateTheme());
        VehicleSearchCriteria? request = null;
        int calls = 0;
        input.SearchRequested += criteria =>
        {
            request = criteria;
            calls++;
        };
        input.Registration.Text = "  ABC123  ";
        input.VehicleType.SetValue("Car");
        input.Color.SetValue("Blue");
        input.Wheels.Text = "4";
        input.Manufacturer.Text = "  Volvo  ";

        // Act
        PressButton(input, "Search");

        // Assert
        Assert.Equal(1, calls);
        Assert.NotNull(request);
        Assert.Equal("ABC123", request.RegistrationNumber);
        Assert.Equal("Car", request.VehicleType);
        Assert.Equal(VehicleColor.Blue, request.Color);
        Assert.Equal(4, request.NumberOfWheels);
        Assert.Equal("Volvo", request.Manufacturer);
        Assert.Null(request.CountryCode);
        Assert.Null(request.ManufacturingYear);
        Assert.Null(request.Model);
        Assert.Empty(input.ErrorMessage);
    }

    private static GarageTheme CreateTheme()
    {
        GarageTheme theme = new();
        theme.Apply();
        return theme;
    }

    private static void PressButton(View root, string text)
    {
        var button = Assert.Single(Descendants(root).OfType<Button>(), candidate =>
            candidate.Text.Replace("_", "").Trim().Equals(text, StringComparison.OrdinalIgnoreCase));
        Assert.True(button.Enabled);
        button.InvokeCommand(Command.Accept);
    }

    private static TextField FindTextInput(View root, string caption)
    {
        var label = Assert.Single(Descendants(root).OfType<Label>(), candidate => candidate.Text == caption);
        Assert.NotNull(label.SuperView);
        return label.SuperView.SubViews.SkipWhile(view => !ReferenceEquals(view, label)).OfType<TextField>().First();
    }

    private static IEnumerable<View> Descendants(View root)
    {
        foreach (View view in root.SubViews)
        {
            yield return view;
            foreach (View nested in Descendants(view))
                yield return nested;
        }
    }
}
