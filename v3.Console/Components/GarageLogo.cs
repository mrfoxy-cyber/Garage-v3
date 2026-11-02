using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using GuiAttribute = Terminal.Gui.Drawing.Attribute;

namespace Garage.Terminal.Components;

public sealed class GarageLogo : AdaptiveComponent
{
    private const int PixelWidth = 2;
    private const string Brand = "GARAGE V3";
    private readonly View picture;
    private readonly Label caption;
    private readonly bool showName;

    private static readonly string[] Pixels = [
        "KKKKKKKKKKKKKKKKKKKKKKKKKK",
        "KBBBBBBBBBBBBBBBBBBBBBBBBK",
        "KBBBKKKKKKKKKKKKKKKKKKBBBK",
        "KBBKKKKKKKKKKKKKKKKKKKKBBK",
        "KBKKKKKKRRRRRRRBBBKKKKKKBK",
        "KBKKKKKBBBBBBBBBBDDKKKKKBK",
        "KBKKKKBVVVVVVVVVBDDDKKKKBK",
        "KBKKKBWWWWWWWWWWBWWBBKKKBK",
        "KBKKKBWWWWWWWGGGBWWWBKKKBK",
        "KBKKKBWWWWWWGGGGBWWWBEKKBK",
        "KBKRRRRRRRRRRRRRBWWWBDEKBK",
        "KBKBBSSSSSSSBBBBBDBBBDEKBK",
        "KBKBBLBSSSSSLBBBBDBBBDEKBK",
        "KBKKBBBSSSSBBBBBDDDDDDEKBK",
        "KBKKKKBBBBBBBBBDDTTNJTKKBK",
        "KBBBBKKHHTTKKKKHHTTKKBBBBK",
        "KKKKKKKHHTTKKKKHHTTKKKKKKK",
        "KKKKKKKKKKKKKKKKKKKKKKKKKK"
    ];

    public override int MinimumWidth => Brand.Length;
    public override int PreferredWidth => Pixels[0].Length * PixelWidth;

    public GarageLogo(GarageTheme theme, bool showName = true, GarageLogoPalette? palette = null)
    {
        this.showName = showName;
        palette ??= new();
        CanFocus = false;
        Width = PreferredWidth;
        Height = Pixels.Length + (showName ? 2 : 0);
        ViewportSettings = ViewportSettingsFlags.Transparent | ViewportSettingsFlags.TransparentMouse;

        picture = new()
        {
            X = Pos.Center(),
            Width = PreferredWidth,
            Height = Pixels.Length,
            CanFocus = false
        };

        Dictionary<char, string> schemes = [];
        foreach (char pixel in Pixels.SelectMany(row => row).Distinct())
        {
            Color color = palette.GetColor(pixel);
            string name = $"Garage.Pixel.{color.R}.{color.G}.{color.B}";
            GuiAttribute paint = new(color, color);
            SchemeManager.AddScheme(name, new Scheme
            {
                Normal = paint,
                Focus = paint,
                HotNormal = paint,
                HotFocus = paint,
                Disabled = paint,
                Editable = paint,
                ReadOnly = paint
            });
            schemes[pixel] = name;
        }

        for (int y = 0; y < Pixels.Length; y++)
        {
            string row = Pixels[y];
            int x = 0;
            while (x < row.Length)
            {
                int start = x;
                char pixel = row[x++];
                while (x < row.Length && row[x] == pixel)
                    x++;

                int width = (x - start) * PixelWidth;
                Label run = new()
                {
                    Text = new string(' ', width),
                    X = start * PixelWidth,
                    Y = y,
                    Width = width,
                    Height = 1,
                    CanFocus = false,
                    SchemeName = schemes[pixel]
                };
                run.TextFormatter.WordWrap = false;
                picture.Add(run);
            }
        }

        caption = new()
        {
            Text = Brand,
            X = Pos.Center(),
            Width = Brand.Length,
            Height = 1,
            CanFocus = false,
            SchemeName = theme.SurfaceScheme
        };
        Add(picture, caption);
        SubViewLayout += (_, _) => ArrangeLogo();
        ArrangeLogo();
    }

    public override int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);
        return availableWidth < PreferredWidth ? 1 : Pixels.Length + (showName ? 2 : 0);
    }

    private void ArrangeLogo()
    {
        bool fits = Viewport.Width >= PreferredWidth;
        picture.Visible = fits;
        caption.Visible = !fits || showName;
        caption.Y = fits ? Pixels.Length + 1 : 0;
    }
}
