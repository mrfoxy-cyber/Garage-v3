using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using GuiAttribute = Terminal.Gui.Drawing.Attribute;

namespace Garage.Terminal.Theming;

public sealed record GarageTheme
{
    public string SurfaceScheme { get; init; } = "Garage.Surface";
    public string GroupScheme { get; init; } = "Garage.Group";
    public string PanelHeaderScheme { get; init; } = "Garage.PanelHeader";
    public string CommandBarScheme { get; init; } = "Garage.Commands";
    public string ErrorScheme { get; init; } = "Garage.Error";

    public void Apply()
    {
        SchemeManager.AddScheme(SurfaceScheme, CreateScheme(18));
        SchemeManager.AddScheme(GroupScheme, CreateScheme(34));
        SchemeManager.AddScheme(PanelHeaderScheme, CreateScheme(52));
        SchemeManager.AddScheme(CommandBarScheme, CreateScheme(44));
        SchemeManager.AddScheme(ErrorScheme, CreateScheme(34));
    }

    private static Scheme CreateScheme(int background)
    {
        return new Scheme
        {
            Normal = new GuiAttribute(Gray(225), Gray(background)),
            HotNormal = new GuiAttribute(Gray(255), Gray(background)),
            Focus = new GuiAttribute(Gray(15), Gray(205)),
            HotFocus = new GuiAttribute(Gray(0), Gray(235)),
            Editable = new GuiAttribute(Gray(245), Gray(background + 12)),
            ReadOnly = new GuiAttribute(Gray(200), Gray(background)),
            Disabled = new GuiAttribute(Gray(120), Gray(background))
        };
    }

    private static Color Gray(int brightness) => new(brightness, brightness, brightness);
}
