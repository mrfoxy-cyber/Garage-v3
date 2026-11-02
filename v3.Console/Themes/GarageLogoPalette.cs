using Terminal.Gui.Drawing;

namespace Garage.Terminal.Theming;

public sealed record GarageLogoPalette
{
    public Color Outline { get; init; } = new(0, 0, 0);
    public Color Frame { get; init; } = new(153, 0, 48);
    public Color Highlight { get; init; } = new(237, 28, 36);
    public Color BodyShade { get; init; } = new(181, 14, 23);
    public Color SideShade { get; init; } = new(117, 9, 44);
    public Color DeepShade { get; init; } = new(92, 6, 35);
    public Color Glass { get; init; } = new(153, 217, 234);
    public Color GlassShade { get; init; } = new(49, 132, 153);
    public Color Reflection { get; init; } = new(255, 255, 255);
    public Color Headlights { get; init; } = new(245, 228, 156);
    public Color Tires { get; init; } = new(70, 70, 70);
    public Color TireHighlight { get; init; } = new(112, 112, 112);
    public Color TireShade { get; init; } = new(31, 31, 31);
    public Color TireEdge { get; init; } = new(74, 74, 74);

    public Color GetColor(char pixel) => pixel switch
    {
        'K' => Outline,
        'B' => Frame,
        'R' => Highlight,
        'S' => BodyShade,
        'D' => SideShade,
        'E' => DeepShade,
        'W' => Glass,
        'V' => GlassShade,
        'G' => Reflection,
        'L' => Headlights,
        'T' => Tires,
        'H' => TireHighlight,
        'N' => TireShade,
        'J' => TireEdge,
        _ => throw new ArgumentOutOfRangeException(nameof(pixel), pixel, "Unknown pixel colour.")
    };
}
