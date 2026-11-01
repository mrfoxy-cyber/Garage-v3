namespace Garage.Terminal.Theming;

public sealed record GarageTheme
{
    public string SurfaceScheme { get; init; } = "Base";
    public string PanelHeaderScheme { get; init; } = "Accent";
    public string CommandBarScheme { get; init; } = "Menu";
    public string ErrorScheme { get; init; } = "Error";
}
