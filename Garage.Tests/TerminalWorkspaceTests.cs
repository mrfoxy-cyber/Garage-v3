using Garage.Services;
using Garage.Terminal.Forms;
using Garage.Terminal.Presentation;
using Garage.Tests.Fixtures;
using Garage.Vehicles;
using Garage.Engines;
using Garage.Vehicles.Types;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Tests;

public class TerminalWorkspaceTests
{
    [Fact]
    public void Workspace_LayoutAndKeyboardTraversalReachTheVehicleTable()
    {
        using var app = global::Terminal.Gui.App.Application.Create();
        GarageService service = new([new VehicleGarage("Test", 2)], [VehicleFixtures.CreateCar()]);
        using GarageWorkspace workspace = new(app, service);
        workspace.BeginInit();
        workspace.EndInit();
        workspace.Layout(new System.Drawing.Size(100, 32));
        workspace.SetFocus();
        HashSet<Type> reached = [];
        List<string> route = [];
        for (int index = 0; index < 40; index++)
        {
            if (workspace.MostFocused is { } focus) { reached.Add(focus.GetType()); route.Add($"{focus.GetType().Name}:{focus.Text}"); }
            workspace.AdvanceWorkspaceFocus();
        }
        Assert.Contains(typeof(global::Terminal.Gui.Views.ListView), reached);
        Assert.True(reached.Contains(typeof(global::Terminal.Gui.Views.TextField)), string.Join(" -> ", route));
        Assert.Contains(typeof(global::Terminal.Gui.Views.TableView), reached);
    }
    [Fact]
    public void FactoryForms_CoverAllVehicleAndEngineTypes()
    {
        Assert.Equal(6, FactoryDefinition.Vehicles.Count);
        Assert.Equal(10, FactoryDefinition.Engines.Count);
        foreach (FactoryDefinition definition in FactoryDefinition.Vehicles.Concat(FactoryDefinition.Engines))
        {
            Assert.NotEmpty(definition.Fields);
            Assert.All(definition.Fields, field => Assert.False(string.IsNullOrWhiteSpace(field.Label)));
            Assert.DoesNotContain(definition.Fields, field => field.Type == typeof(List<Engine>));
        }
    }

    [Fact]
    public void FactoryForm_MapsValidCarValuesThroughDomainFactory()
    {
        FactoryDefinition car = FactoryDefinition.Vehicles[0];
        Dictionary<string, object?> values = new()
        {
            ["registrationNumber"] = "NEW123", ["countryCode"] = CountryCode.SWE,
            ["manufacturer"] = "Volvo", ["model"] = "V70", ["manufacturingYear"] = 2018,
            ["color"] = Garage.Vehicles.Types.VehicleColor.Black,
            ["weightKg"] = 1700d, ["length"] = 4.8d, ["width"] = 1.8d, ["height"] = 1.5d,
            ["vin"] = "VOLVO-NEW-001", ["bodyType"] = Garage.Vehicles.Types.BodyType.Sedan,
            ["numberOfDoors"] = 5, ["numberOfSeats"] = 5
        };
        Car vehicle = Assert.IsType<Car>(car.Create(values, [EngineFixtures.CreateEngine()]));
        Assert.Equal("NEW123", vehicle.RegistrationNumber);
        Assert.Single(vehicle.SpecificationDetails.Engines);
        values["length"] = -1d;
        Assert.ThrowsAny<ArgumentException>(() => car.Create(values));
    }

    [Fact]
    public void VehicleForms_CreateEverySupportedVehicleType()
    {
        Dictionary<string, object?> values = new()
        {
            ["registrationNumber"] = "ABC123", ["countryCode"] = CountryCode.SWE,
            ["manufacturer"] = "Test", ["model"] = "Model", ["manufacturingYear"] = 2020,
            ["color"] = VehicleColor.Black, ["weightKg"] = 1500d,
            ["length"] = 4.8d, ["width"] = 1.8d, ["height"] = 1.5d,
            ["lengthMeters"] = 4.8d, ["widthMeters"] = 1.8d, ["heightMeters"] = 1.5d,
            ["vin"] = "TEST-VIN", ["hin"] = "TEST-HIN",
            ["airframeSerialNumber"] = "TEST-AIRFRAME",
            ["bodyType"] = BodyType.Sedan, ["motorcycleType"] = MotorcycleType.Touring,
            ["boatType"] = BoatType.Motorboat, ["numberOfDoors"] = 4,
            ["numberOfSeats"] = 4, ["numberOfWheels"] = 4, ["numberOfAxles"] = 2,
            ["hasSidecar"] = false, ["passengerCapacity"] = 2, ["driverSeats"] = 1,
            ["passengerSeatCount"] = 2, ["standingPassengerCapacity"] = 10,
            ["driverSeatCount"] = 1, ["draftMeters"] = 0.5d, ["hasCabin"] = false,
            ["cabinPassengerSeatCount"] = 0, ["cabinCrewSeatCount"] = 0,
            ["wingspanMeters"] = 10d, ["crewSeatCount"] = 0,
            ["pilotSeatCount"] = 1, ["hasDualControls"] = false,
            ["maximumAltitudeMeters"] = 4200d, ["maximumLoadKg"] = 1000d
        };

        foreach (FactoryDefinition factory in FactoryDefinition.Vehicles)
        {
            values["registrationNumber"] = factory.Name switch
            {
                "Boat" => "SFB-2700",
                "Airplane" => "SE-AIR",
                _ => "ABC123"
            };
            values["numberOfWheels"] = factory.Name == "Motorcycle" ? 2 : 4;

            Vehicle vehicle = Assert.IsAssignableFrom<Vehicle>(
                factory.Create(values, [EngineFixtures.CreateEngine()]));
            Assert.Equal(factory.Name, vehicle.GetType().Name);
        }
    }

    [Fact]
    public void EngineForms_MapEverySupportedEngineFactory()
    {
        Dictionary<string, object?> values = new()
        {
            ["serialNumber"] = "ENGINE-TEST", ["model"] = "Test engine", ["fuelType"] = FuelType.Gasoline,
            ["horsePower"] = 150, ["displacementCc"] = 2000, ["cylinderCount"] = 4, ["rotorCount"] = 2,
            ["powerKw"] = 75d, ["voltage"] = 400d, ["boilerPressureBar"] = 20d,
            ["thrustKn"] = 10d, ["bypassRatio"] = 4d, ["shaftHorsePower"] = 1000
        };
        foreach (FactoryDefinition factory in FactoryDefinition.Engines)
        {
            foreach (FieldDefinition field in factory.Fields)
                if (values.TryGetValue(field.Name, out object? value) && value is IConvertible)
                    values[field.Name] = field.Type.IsEnum ? value : Convert.ChangeType(value, field.Type);
            Engine engine = Assert.IsAssignableFrom<Engine>(factory.Create(values));
            Assert.Equal("ENGINE-TEST", engine.SerialNumber);
        }
    }

    [Fact]
    public void Fields_ParseOptionalEnumsAndFiniteMeasurements()
    {
        Assert.Null(new FieldDefinition("year", "Year", typeof(int?), false).Parse(""));
        Assert.Equal(2.5d, new FieldDefinition("width", "Width", typeof(double)).Parse("2.5"));
        Assert.Equal(true, new FieldDefinition("sidecar", "Sidecar", typeof(bool)).Parse("Yes"));
        Assert.Throws<ArgumentException>(() => new FieldDefinition("width", "Width", typeof(double)).Parse("NaN"));
        Assert.Throws<ArgumentException>(() => new FieldDefinition("name", "Name", typeof(string)).Parse(" "));
    }

    [Fact]
    public void Dashboard_FiltersCatalogAndRetainsGarageLocations()
    {
        Vehicle parked = VehicleFixtures.CreateCar("ABC123");
        Vehicle available = VehicleFixtures.CreateBoat();
        VehicleGarage garage = new("First", 2, [parked]);
        GarageService service = new([garage], [available]);
        DashboardState state = new(service) { Page = WorkspacePage.Vehicles, QuickFilter = "abc" };
        Assert.Same(parked, Assert.Single(state.VisibleVehicles()));
        Assert.Equal(1, state.Spot(parked));
        Assert.Null(state.Location(available));
        state.Page = WorkspacePage.Parking;
        state.QuickFilter = "";
        Assert.Same(parked, Assert.Single(state.VisibleVehicles()));
        state.Page = WorkspacePage.Search;
        state.Criteria = new() { Manufacturer = "missing" };
        Assert.Empty(state.VisibleVehicles());
    }
}
