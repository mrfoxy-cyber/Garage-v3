using Garage.Terminal.Theming;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class BottomCommandBar : View
{
    private sealed record Entry(GarageMenuAction Action, string Label, Shortcut Control);

    private readonly List<Entry> entries = [];
    private readonly List<Entry> overflow = [];
    private readonly Shortcut more;
    private readonly GarageTheme theme;

    public event Action<GarageMenuAction>? OptionRequested;

    public BottomCommandBar(GarageTheme theme)
    {
        this.theme = theme;

        CanFocus = true;
        Width = Dim.Fill();
        Height = 1;
        Y = Pos.AnchorEnd(1);
        SchemeName = theme.CommandBarScheme;

        AddOption(GarageMenuAction.ParkVehicle, "Park", Key.F2);
        AddOption(GarageMenuAction.RemoveVehicle, "Remove", Key.F3);
        AddOption(GarageMenuAction.FindVehicle, "Find", Key.F4);
        AddOption(GarageMenuAction.SearchVehicles, "Search", Key.F6);
        AddOption(GarageMenuAction.Refresh, "Refresh", Key.F5);
        AddOption(GarageMenuAction.SelectGarage, "Garages", Key.F7);
        AddOption(GarageMenuAction.ShowTypeCounts, "Types", Key.F9);
        AddOption(GarageMenuAction.CreateGarage, "New garage", Key.F1);
        AddOption(GarageMenuAction.Quit, "Quit", Key.F10);

        more = new Shortcut(Key.F12, "More", ShowMore)
        {
            BindKeyToApplication = true,
            Height = 1
        };

        Add(more);

        SubViewLayout += (_, _) => ArrangeCommands();
    }

    private void AddOption(GarageMenuAction action, string label, Key key)
    {
        Shortcut control = new(key, label, () => OptionRequested?.Invoke(action))
        {
            BindKeyToApplication = true,
            Height = 1
        };

        entries.Add(new(action, label, control));
        Add(control);
    }

    private void ArrangeCommands()
    {
        int availableWidth = Viewport.Width;

        if (availableWidth <= 0)
            return;

        overflow.Clear();

        int totalWidth = entries.Sum(entry => RequiredWidth(entry.Control) + 1);
        bool needsMore = totalWidth > availableWidth;

        int moreWidth = Math.Min(RequiredWidth(more), availableWidth);
        int budget = needsMore ? Math.Max(0, availableWidth - moreWidth - 1) : availableWidth;
        int left = 0;

        foreach (var entry in entries)
        {
            int width = RequiredWidth(entry.Control);
            bool fits = left + width <= budget;

            entry.Control.Visible = fits;

            if (!fits)
            {
                overflow.Add(entry);
                continue;
            }

            entry.Control.X = left;
            entry.Control.Width = width;

            left += width + 1;
        }

        more.Visible = overflow.Count > 0;
        more.X = left;
        more.Width = Math.Max(1, availableWidth - left);
    }

    private void ShowMore()
    {
        if (overflow.Count == 0)
            return;

        GarageMenuAction? selected = null;

        using Window popup = new()
        {
            Title = "More actions",
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Auto(DimAutoStyle.Content, maximumContentDim: Dim.Percent(100)),
            Height = Dim.Auto(DimAutoStyle.Content, maximumContentDim: Dim.Percent(100)),
            SchemeName = theme.CommandBarScheme
        };

        int row = 0;

        foreach (var entry in overflow.ToArray())
        {
            Shortcut option = new(entry.Control.Key, entry.Label, () =>
            {
                selected = entry.Action;
                App!.RequestStop();
            })
            {
                X = 0,
                Y = row++,
                Width = Dim.Auto(),
                Height = 1
            };

            popup.Add(option);
        }

        Button close = new() { Text = "_Close", Y = row + 1 };
        close.Accepted += (_, _) => App!.RequestStop();
        popup.Add(close);

        App!.Run(popup);

        if (selected is { } action)
            OptionRequested?.Invoke(action);
    }

    private static int RequiredWidth(Shortcut shortcut)
    {
        string text = $"{shortcut.Key} {shortcut.Title}";
        return TextFormatter.GetWidestLineLength(text, 4) + 4;
    }
}
