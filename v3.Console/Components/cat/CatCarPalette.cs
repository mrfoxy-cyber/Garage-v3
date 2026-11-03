using Terminal.Gui.Drawing;

namespace Garage.Terminal.Theming;

public sealed record CatCarPalette
{
    public Color Body { get; init; } = new(153, 0, 48);
    public Color Highlight { get; init; } = new(237, 28, 36);
    public Color Glass { get; init; } = new(153, 217, 234);
    public Color Cat { get; init; } = new(70, 70, 70);
    public Color Wheel { get; init; } = new(156, 90, 60);
    public Color Smoke { get; init; } = new(245, 228, 156);

    public IReadOnlyDictionary<char, Color> CreateColors() => new Dictionary<char, Color>
    {
        ['B'] = Body,
        ['R'] = Highlight,
        ['W'] = Glass,
        ['C'] = Cat,
        ['H'] = Wheel,
        ['L'] = Smoke,
        ['K'] = new(0, 0, 0),
        ['D'] = new(117, 9, 44),
        ['S'] = new(181, 14, 23),
        ['T'] = new(41, 41, 41),
        ['V'] = new(132, 179, 191),
        ['U'] = new(69, 136, 153),
        ['P'] = new(255, 163, 177),
        ['Y'] = new(255, 242, 0),
        ['G'] = new(255, 255, 255)
    };
}
