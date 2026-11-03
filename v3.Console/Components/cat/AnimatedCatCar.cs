using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Components;

public sealed class AnimatedCatCar : AnimatedPixelComponent
{
    public AnimatedCatCar(GarageTheme theme, int frameMilliseconds = 160, CatCarPalette? palette = null)
        : base(CatCarFrames.Create(), (palette ?? new()).CreateColors(), "GARAGE V3", theme.SurfaceScheme, frameMilliseconds)
    {
    }
}
