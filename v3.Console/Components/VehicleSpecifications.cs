using Garage.Terminal.Components.Shared;
using Garage.Terminal.Presentation;
using Garage.Terminal.Theming;
using Vehicles.Interfaces;

namespace Garage.Terminal.Components;

public sealed class VehicleSpecifications : AdaptiveText
{
    private readonly VehicleSpecificationReader reader;

    public VehicleSpecifications(GarageTheme theme, VehicleSpecificationReader? reader = null) : base(theme)
    {
        this.reader = reader ?? new();
        ShowText("Select a vehicle to see its details.");
    }

    public void ShowVehicle(IVehicle? vehicle)
    {
        if (vehicle is null)
        {
            ShowText("Select a vehicle to see its details.");
            return;
        }

        var sections = reader.Read(vehicle);
        string text = string.Join("\n\n", sections.Select(section => $"{section.Title.ToUpperInvariant()}\n{string.Join('\n', section.Lines)}"));
        ShowText(text.Length == 0 ? "No additional specifications supplied." : text);
    }
}
