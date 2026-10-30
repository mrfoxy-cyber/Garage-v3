using Garage.Engines;
using Garage.Results;
using Garage.Services;
using Garage.Terminal.Forms;
using Garage.Vehicles;
using Garage.Vehicles.Components;
using VehicleGarage = Garage.Models.Garage;

namespace Garage.Terminal.Presentation;

internal sealed class Workflows(Dialogs dialogs, GarageService service)
{
    public VehicleCreationResult? CreateVehicle(VehicleGarage? garage = null, int? spotNumber = null)
    {
        int? type = dialogs.Choose("Create a vehicle / Choose type", FactoryDefinition.Vehicles.Select(f => f.Name).ToList());
        if (type is null) return null;
        FactoryDefinition factory = FactoryDefinition.Vehicles[type.Value];
        Dictionary<string, object?>? values = null;
        List<Engine> engines = [];
        while (true)
        {
            values = dialogs.Form($"New {factory.Name.ToLowerInvariant()}", factory.Fields, "_Engines", values);
            if (values is null) return null;
            while (true)
            {
                int? action = dialogs.Choose($"{factory.Name} / {engines.Count} engine(s)",
                    ["Add an engine", "Review engines", "Remove an engine", "Create vehicle", "Edit vehicle fields", "Cancel"], 3);
                if (action is null or 5) return null;
                if (action == 4) break;
                if (action == 0)
                {
                    Engine? engine = CreateEngine();
                    if (engine is not null) engines.Add(engine);
                }
                else if (action == 1) dialogs.Message("Draft engines", VehicleDetails.Engines(engines));
                else if (action == 2)
                {
                    int? remove = dialogs.Choose("Remove draft engine", engines.Select(e => $"{e.GetType().Name} / {e.Model} / {e.SerialNumber}").ToList());
                    if (remove is int index) engines.RemoveAt(index);
                }
                else if (action == 3)
                {
                    VehicleCreationResult result = service.VehicleCreation.Create(() => (Vehicle)factory.Create(values, engines), garage, spotNumber);
                    if (result.Success) return result;
                    dialogs.Message("Check the vehicle", result.Message + "\n\nYour entries are kept. Choose Edit vehicle fields to correct them.");
                }
            }
        }
    }

    private Engine? CreateEngine()
    {
        int? kind = dialogs.Choose("Add an engine / Choose type", FactoryDefinition.Engines.Select(f => f.Name).ToList());
        if (kind is null) return null;
        FactoryDefinition factory = FactoryDefinition.Engines[kind.Value];
        Dictionary<string, object?>? values = null;
        while (true)
        {
            values = dialogs.Form($"New {factory.Name.ToLowerInvariant()} engine", factory.Fields, "_Add engine", values);
            if (values is null) return null;
            try { return (Engine)factory.Create(values); }
            catch (Exception exception) when (exception is ArgumentException or NotSupportedException)
            { dialogs.Message("Check the engine", exception.Message); }
        }
    }

    public GarageCreationResult? CreateGarage()
    {
        List<FieldDefinition> fields =
        [
            new("name", "Garage name", typeof(string)),
            new("capacity", "Number of parking spots", typeof(int), Initial: "20")
        ];
        Dictionary<string, object?>? values = dialogs.Form("New garage", fields);
        if (values is null) return null;
        string name = (string)values["name"]!;
        int capacity = (int)values["capacity"]!;
        if (capacity < 1 || capacity > VehicleGarage.MaximumGarageCapacity)
        { dialogs.Message("Check capacity", $"Choose between 1 and {VehicleGarage.MaximumGarageCapacity} spots."); return null; }
        int? mode = dialogs.Choose("Parking spot sizes", ["Unrestricted / no size limit", "Set dimensions and customize individual spots"]);
        if (mode is null) return null;
        if (mode == 0) return service.CreateGarage(name, capacity);
        Dimensions? defaultSize = ReadDimensions("Default parking spot size");
        if (defaultSize is null) return null;
        List<Dimensions> sizes = Enumerable.Repeat(defaultSize, capacity).ToList();
        while (true)
        {
            int? action = dialogs.Choose("Parking layout", ["Create garage", "Customize a spot", "Cancel"]);
            if (action is null or 2) return null;
            if (action == 0) return service.CreateGarage(name, sizes);
            Dictionary<string, object?>? spot = dialogs.Form("Customize parking spot",
                [new("number", $"Spot number (1 - {capacity})", typeof(int))]);
            if (spot is null) continue;
            int number = (int)spot["number"]!;
            if (number < 1 || number > capacity) { dialogs.Message("Check spot number", $"Choose a spot between 1 and {capacity}."); continue; }
            Dimensions? size = ReadDimensions($"Size for spot {number}");
            if (size is not null) sizes[number - 1] = size;
        }
    }

    private Dimensions? ReadDimensions(string title)
    {
        Dictionary<string, object?>? values = null;
        while (true)
        {
            values = dialogs.Form(title,
            [new("length", "Length (m)", typeof(double), Initial: "5"),
             new("width", "Width (m)", typeof(double), Initial: "2.5"),
             new("height", "Height (m)", typeof(double), Initial: "2.2")], initial: values);
            if (values is null) return null;
            try { return new Dimensions((double)values["length"]!, (double)values["width"]!, (double)values["height"]!); }
            catch (ArgumentException exception) { dialogs.Message("Check dimensions", exception.Message); }
        }
    }

    public GarageCreationResult? ImportGarage()
    {
        string? configuration = dialogs.JsonFile("Open garage layout JSON");
        if (configuration is null) return null;
        int? mode = dialogs.Choose("Import garage", ["Create an empty garage from this layout", "Also load parking assignments", "Cancel"]);
        if (mode is null or 2) return null;
        string? placement = mode == 1 ? dialogs.JsonFile("Open parking assignments JSON") : null;
        if (mode == 1 && placement is null) return null;
        return service.CreateGarageFromJson(configuration, placement);
    }
}
