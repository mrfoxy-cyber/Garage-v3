using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Garage_V3.results;

namespace Garage.Terminal.Components;

public sealed class OperationFeedback : AdaptiveText
{
    private readonly GarageTheme theme;

    public OperationFeedback(GarageTheme theme) : base(theme)
    {
        this.theme = theme;
        Clear();
    }

    public void ShowResult(OperationResult result) => ShowMessage(result.Success, result.Message);
    public void ShowResult(ParkingResult result) => ShowMessage(result.Success, result.Message);

    public void ShowMessage(bool success, string message)
    {
        SchemeName = success ? theme.SurfaceScheme : theme.ErrorScheme;
        ShowText($"{(success ? "OK" : "Error")}: {message}");
    }

    public void Clear()
    {
        SchemeName = theme.SurfaceScheme;
        ShowText("Ready.");
    }
}
