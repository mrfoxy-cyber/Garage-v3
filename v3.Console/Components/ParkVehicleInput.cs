using Garage.Terminal.Components.Shared;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;
using Garage.Vehicles;
using Garage.Vehicles.Types;

namespace Garage.Terminal.Components;

public sealed class ParkVehicleInput : FormComponent
{
    private readonly ParkingOptionsInput parkingOptions;
    private readonly VehicleDetailsInput vehicleDetails;
    private readonly VehicleIdentityInput identityInput;

    private readonly CollapsiblePanel<VehicleIdentityInput> identityPanel;
    private readonly CollapsiblePanel<VehicleDetailsInput> detailsPanel;
    private readonly CollapsiblePanel<ParkingOptionsInput> parkingPanel;

    public event Action<ParkVehicleRequest>? ParkRequested;

    public ParkVehicleInput(GarageTheme theme) : base(theme)
    {
        identityInput = new(theme);
        vehicleDetails = new(theme);
        parkingOptions = new(theme);

        identityPanel = AddSection("Required information", identityInput, theme);
        detailsPanel = AddSection("Optional vehicle details", vehicleDetails, theme, expanded: false);
        parkingPanel = AddSection("Parking options", parkingOptions, theme, expanded: false);

        AddAction("_Park vehicle", Submit);
    }

    public void SetAllowedTypes(IEnumerable<string> types) => identityInput.SetAllowedTypes(types);

    public void Submit()
    {
        ShowError(null);
        identityInput.ShowError(null);
        vehicleDetails.ShowError(null);
        parkingOptions.ShowError(null);

        string registration = identityInput.Registration.Text.Trim();

        if (registration.Length == 0)
        {
            ShowSectionError(identityPanel, "Enter a registration number.");
            return;
        }

        if (identityInput.VehicleType.Value.Length == 0)
        {
            ShowSectionError(identityPanel, "Choose a vehicle type.");
            return;
        }

        if (!Enum.TryParse(identityInput.Country.Value, out CountryCode country) ||
            country == CountryCode.Unspecified || !Enum.IsDefined(country))
        {
            ShowSectionError(identityPanel, "Choose a supported country.");
            return;
        }

        if (!TryOptionalInteger(vehicleDetails.Wheels.Text, out int? wheels) || wheels < 0)
        {
            ShowSectionError(detailsPanel, "Wheel count must be a non-negative whole number, or blank.");
            return;
        }

        if (!TryOptionalInteger(vehicleDetails.ManufacturingYear.Text, out int? year) || year <= 0)
        {
            ShowSectionError(detailsPanel, "Manufacturing year must be a positive whole number, or blank.");
            return;
        }

        if (!TryOptionalInteger(parkingOptions.Spot.Text, out int? spot) || spot < 1)
        {
            ShowSectionError(parkingPanel, "Spot must be a positive whole number, or blank.");
            return;
        }

        VehicleColor? color = vehicleDetails.Color.Value == "Unknown"
            ? null
            : Enum.Parse<VehicleColor>(vehicleDetails.Color.Value);

        ParkRequested?.Invoke(new ParkVehicleRequest(
            VehicleType: identityInput.VehicleType.Value,
            RegistrationNumber: registration,
            CountryCode: country,
            Color: color,
            SpotNumber: spot,
            NumberOfWheels: wheels,
            Manufacturer: Optional(vehicleDetails.Manufacturer.Text),
            Model: Optional(vehicleDetails.Model.Text),
            ManufacturingYear: year));
    }

    public void Reset()
    {
        identityInput.Reset();
        vehicleDetails.Reset();
        parkingOptions.Reset();
        ShowError(null);
    }

    private static void ShowSectionError<T>(CollapsiblePanel<T> panel, string message) where T : FormComponent
    {
        panel.Content.ShowError(message);
        panel.SetExpanded(true);
    }

    private static string? Optional(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    private static bool TryOptionalInteger(string text, out int? value)
    {
        value = null;

        if (string.IsNullOrWhiteSpace(text))
            return true;

        if (!int.TryParse(text, out int number))
            return false;

        value = number;
        return true;
    }
}
