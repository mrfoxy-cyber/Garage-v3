using Garage.Terminal.Components.Shared;
using Garage.Terminal.Theming;
using Garage_V3.queries;

namespace Garage.Terminal.Components;

public sealed class VehicleTypeCounts : AdaptiveText
{
    public VehicleTypeCounts(GarageTheme theme) : base(theme)
    {
        ShowText("No vehicle types to show.");
    }

    public void ShowCounts(IReadOnlyList<VehicleTypeCount> counts)
    {
        ArgumentNullException.ThrowIfNull(counts);
        if (counts.Count == 0)
        {
            ShowText("No vehicles are parked.");
            return;
        }

        int labelWidth = counts.Max(item => ComponentMetrics.TextWidth(item.VehicleType));
        ShowText(string.Join('\n', counts.OrderBy(item => item.VehicleType).Select(item => $"{item.VehicleType.PadRight(labelWidth)}  {item.Count}")));
    }
}
