using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using GuiAttribute = Terminal.Gui.Drawing.Attribute;

namespace Garage.Terminal.Theming;

public sealed record GarageTheme
{
    public string SurfaceScheme { get; init; } = "Garage.Surface";
    public string GroupScheme { get; init; } = "Garage.Group";
    public string PanelHeaderScheme { get; init; } = "Garage.PanelHeader";
    public string CommandBarScheme { get; init; } = "Garage.Commands";
    public string ErrorScheme { get; init; } = "Garage.Error";
    public string InputScheme { get; init; } = "Garage.Input";
    public string ButtonScheme { get; init; } = "Garage.Button";
    public string DangerButtonScheme { get; init; } = "Garage.DangerButton";

    private static readonly Color White = new(255, 255, 255);

    private static readonly Color Light_Grey = new(225, 225, 225);
    private static readonly Color Ink = new(30, 41, 59);
    private static readonly Color Muted = new(100, 116, 139);
    private static readonly Color Blue = new(29, 78, 216);
    private static readonly Color DeepBlue = new(30, 64, 175);
    private static readonly Color PaleBlue = new(219, 234, 254);
    private static readonly Color Group = new(245, 248, 253);
    private static readonly Color Input = new(237, 242, 249);
    private static readonly Color Disabled = new(226, 232, 240);
    private static readonly Color ErrorText = new(153, 27, 27);
    private static readonly Color ErrorBackground = new(254, 242, 242);

    public void Apply()
    {
        SchemeManager.AddScheme(SurfaceScheme, CreateSurface(White));
        SchemeManager.AddScheme(GroupScheme, CreateSurface(Group));
        SchemeManager.AddScheme(PanelHeaderScheme, CreateSurface(PaleBlue, DeepBlue));
        SchemeManager.AddScheme(CommandBarScheme, CreateFilled(DeepBlue, White, DeepBlue));
        SchemeManager.AddScheme(ErrorScheme, CreateSurface(ErrorBackground, ErrorText));
        SchemeManager.AddScheme(InputScheme, CreateSurface(Input, focusedBackground: PaleBlue, focusedForeground: Ink));
        SchemeManager.AddScheme(ButtonScheme, CreateFilled(Blue, PaleBlue, DeepBlue));
        SchemeManager.AddScheme(DangerButtonScheme, CreateFilled(new Color(185, 28, 28), ErrorBackground, ErrorText));

        Button.DefaultShadow = ShadowStyles.None;
    }

    public void StyleButton(Button button, bool danger = false)
    {
        button.SchemeName = danger ? DangerButtonScheme : ButtonScheme;
        button.ShadowStyle = ShadowStyles.Opaque;
        button.NoDecorations = true;
        button.NoPadding = false;
        button.Padding.Thickness = new Thickness(1, 0, 1, 0);
    }

    public void StyleChoice(Button button)
    {
        StyleButton(button);
        button.SchemeName = InputScheme;
    }

    private static Scheme CreateSurface(Color background, Color? foreground = null, Color? focusedBackground = null, Color? focusedForeground = null)
    {
        Color text = foreground ?? Ink;
        Color focusBackground = focusedBackground ?? Blue;
        Color focusText = focusedForeground ?? White;

        return new Scheme
        {
            Normal = new GuiAttribute(text, background),
            HotNormal = new GuiAttribute(DeepBlue, background),
            Focus = new GuiAttribute(focusText, focusBackground),
            HotFocus = new GuiAttribute(focusText, focusBackground),
            Active = new GuiAttribute(Ink, PaleBlue),
            HotActive = new GuiAttribute(DeepBlue, PaleBlue),
            Highlight = new GuiAttribute(Ink, PaleBlue),
            Editable = new GuiAttribute(text, Light_Grey),
            ReadOnly = new GuiAttribute(text, background),
            Disabled = new GuiAttribute(Muted, background)
        };
    }

    private static Scheme CreateFilled(Color background, Color focusedBackground, Color? focusedForeground = null)
    {
        Color focusText = focusedForeground ?? White;

        return new Scheme
        {
            Normal = new GuiAttribute(White, background),
            HotNormal = new GuiAttribute(White, background),
            Focus = new GuiAttribute(focusText, focusedBackground),
            HotFocus = new GuiAttribute(focusText, focusedBackground),
            Active = new GuiAttribute(focusText, focusedBackground),
            HotActive = new GuiAttribute(focusText, focusedBackground),
            Highlight = new GuiAttribute(focusText, focusedBackground),
            Editable = new GuiAttribute(Light_Grey, background),
            ReadOnly = new GuiAttribute(White, background),
            Disabled = new GuiAttribute(Muted, Disabled)
        };
    }
}
