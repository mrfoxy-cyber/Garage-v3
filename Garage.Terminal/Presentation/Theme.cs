using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace Garage.Terminal.Presentation;

internal static class Theme
{
    public static readonly Scheme Surface = Create("#dce5f5", "#101827", "#245e85");
    public static readonly Scheme Panel = Create("#dce5f5", "#162235", "#245e85");
    public static readonly Scheme Accent = Create("#69e0d6", "#101827", "#164b60");
    public static readonly Scheme Muted = Create("#9aaec8", "#101827", "#164b60");
    public static readonly Scheme Danger = Create("#ffb4aa", "#101827", "#713b45");
    public static readonly Scheme Input = Create("#edf5ff", "#243850", "#164b60");

    private static Scheme Create(string text, string background, string selection) => new()
    {
        Normal = new Attribute(new Color(text), new Color(background)),
        Focus = new Attribute(new Color("#ffffff"), new Color(selection)),
        HotNormal = new Attribute(new Color("#69e0d6"), new Color(background)),
        HotFocus = new Attribute(new Color("#ffffff"), new Color(selection)),
        HotActive = new Attribute(new Color("#ffffff"), new Color(selection)),
        Active = new Attribute(new Color("#ffffff"), new Color(selection)),
        Highlight = new Attribute(new Color("#69e0d6"), new Color(selection)),
        Editable = new Attribute(new Color("#ffffff"), new Color(background)),
        ReadOnly = new Attribute(new Color(text), new Color(background)),
        Disabled = new Attribute(new Color("#71849c"), new Color(background))
    };

    private static void ApplyScheme(View view, Scheme? scheme)
    {
        view.SetScheme(scheme ?? Surface);
    }

    public static View Apply(View view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static Button Apply(Button view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static Label Apply(Label view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static Window Apply(Window view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static OpenDialog Apply(OpenDialog view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static FrameView Apply(FrameView view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static ListView Apply(ListView view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static TextField Apply(TextField view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static TableView Apply(TableView view, Scheme? scheme = null)
    { ApplyScheme(view, scheme); return view; }

    public static Button Button(string text, Action action)
    {
        Button button = Apply(new Button { Text = text, ShadowStyle = ShadowStyles.None });
        button.Accepted += (_, e) => { e.Handled = true; action(); };
        return button;
    }
}
