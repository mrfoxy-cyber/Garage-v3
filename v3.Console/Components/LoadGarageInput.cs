using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Components;

public sealed class LoadGarageInput : FormComponent
{
    public event Action? LoadRequested;

    public LoadGarageInput(GarageTheme theme) : base(theme)
    {
        AddAction("_Load garage", () => LoadRequested?.Invoke());
    }
}
