using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using GuiAttribute = Terminal.Gui.Drawing.Attribute;

namespace Garage.Terminal.Components.Shared;

public abstract class AnimatedPixelComponent : AdaptiveComponent
{
    private readonly View scene;
    private readonly Label fallback;
    private readonly List<View> frames = [];
    private readonly int columns;
    private readonly int rows;
    private readonly TimeSpan interval;

    private IApplication? timerApp;
    private object? timer;
    private int frameIndex;

    public override int MinimumWidth => fallback.Text.Length;
    public override int PreferredWidth => columns * 2;
    public int CurrentFrame => frameIndex;

    protected AnimatedPixelComponent(IReadOnlyList<string[]> patterns, IReadOnlyDictionary<char, Color> palette, string fallbackText, string schemeName, int frameMilliseconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(frameMilliseconds);

        if (patterns.Count == 0 || patterns[0].Length == 0)
            throw new ArgumentException("Supply at least one nonempty pixel frame.", nameof(patterns));

        columns = patterns[0][0].Length;
        rows = patterns[0].Length;
        interval = TimeSpan.FromMilliseconds(frameMilliseconds);

        CanFocus = false;
        Width = PreferredWidth;
        Height = rows;
        SchemeName = schemeName;

        scene = new()
        {
            X = Pos.Center(),
            Width = PreferredWidth,
            Height = rows,
            CanFocus = false,
            SchemeName = schemeName
        };

        fallback = new()
        {
            Text = fallbackText,
            X = Pos.Center(),
            Width = fallbackText.Length,
            Height = 1,
            CanFocus = false,
            SchemeName = schemeName
        };

        Dictionary<char, string> schemes = [];

        foreach (var entry in palette)
        {
            Color color = entry.Value;
            string name = $"Garage.Animation.{color.R}.{color.G}.{color.B}";
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

            schemes.Add(entry.Key, name);
        }

        foreach (string[] pattern in patterns)
        {
            if (pattern.Length != rows || pattern.Any(row => row.Length != columns))
                throw new ArgumentException("All frames must have equal dimensions.", nameof(patterns));

            View frame = new()
            {
                Width = PreferredWidth,
                Height = rows,
                Visible = frames.Count == 0,
                CanFocus = false,
                SchemeName = schemeName
            };

            for (int y = 0; y < rows; y++)
            {
                int x = 0;

                while (x < columns)
                {
                    int start = x;
                    char pixel = pattern[y][x++];

                    while (x < columns && pattern[y][x] == pixel)
                        x++;

                    if (pixel == ' ')
                        continue;

                    int width = (x - start) * 2;

                    Label run = new()
                    {
                        Text = new string(' ', width),
                        X = start * 2,
                        Y = y,
                        Width = width,
                        Height = 1,
                        CanFocus = false,
                        SchemeName = schemes[pixel]
                    };

                    run.TextFormatter.WordWrap = false;
                    frame.Add(run);
                }
            }

            frames.Add(frame);
            scene.Add(frame);
        }

        Add(scene, fallback);

        SubViewLayout += (_, _) =>
        {
            bool fits = Viewport.Width >= PreferredWidth;
            scene.Visible = fits;
            fallback.Visible = !fits;
            StartTimer();
        };

        Removed += (_, _) => StopTimer();
        Disposing += (_, _) => StopTimer();
    }

    public override int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);
        return availableWidth >= PreferredWidth ? rows : 1;
    }

    private void StartTimer()
    {
        if (timer is not null || App is null)
            return;

        timerApp = App;

        timer = timerApp.AddTimeout(interval, () =>
        {
            if (App != timerApp)
            {
                timer = null;
                timerApp = null;
                return false;
            }

            if (IsShown())
            {
                frames[frameIndex].Visible = false;
                frameIndex = (frameIndex + 1) % frames.Count;
                frames[frameIndex].Visible = true;
                scene.SetNeedsDraw();
            }

            return true;
        });
    }

    private bool IsShown()
    {
        if (!scene.Visible)
            return false;

        for (View? view = this; view is not null; view = view.SuperView)
        {
            if (!view.Visible)
                return false;
        }

        return true;
    }

    private void StopTimer()
    {
        if (timer is not null)
            timerApp?.RemoveTimeout(timer);

        timer = null;
        timerApp = null;
    }
}
