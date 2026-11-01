using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using Garage.Terminal.Theming;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class CollapsiblePanel<T> : View, IAdaptiveComponent where T : View, IAdaptiveComponent
{
    private const int HeaderHeight = 2;
    private const int ContentInset = 1;

    private readonly string title;
    private readonly Button toggleButton;

    public T Content { get; }
    public bool IsExpanded { get; private set; }

    public event Action? SizeHintsChanged;

    private int ToggleWidth => TextWidth(IsExpanded ? "Collapse" : "Expand") + 6;

    public int MinimumWidth => Math.Max(ToggleWidth + 2, IsExpanded ? Content.MinimumWidth + 2 : 0);
    public int PreferredWidth => Math.Max(TextWidth(title) + ToggleWidth + 2, IsExpanded ? Content.PreferredWidth + 2 : MinimumWidth);

    public CollapsiblePanel(string title, T content, GarageTheme theme, bool expanded = true)
    {
        this.title = title;
        Content = content;
        IsExpanded = expanded;

        CanFocus = true;
        SchemeName = theme.SurfaceScheme;

        View header = new()
        {
            CanFocus = true,
            Width = Dim.Fill(),
            Height = HeaderHeight,
            SchemeName = theme.PanelHeaderScheme
        };

        toggleButton = new()
        {
            Text = expanded ? "<" : ">",
            X = Pos.AnchorEnd(),
            Width = Dim.Auto(),
            Height = Dim.Auto()
        };

        toggleButton.TextFormatter.WordWrap = false;
        toggleButton.Accepted += (_, _) => Toggle();

        Label titleLabel = new()
        {
            Text = title,
            Width = Dim.Fill(toggleButton) - 1,
            Height = 1
        };

        titleLabel.TextFormatter.WordWrap = false;

        header.Add(titleLabel, toggleButton);

        content.X = ContentInset;
        content.Y = HeaderHeight;
        content.Width = Dim.Fill(ContentInset);
        content.Height = Dim.Func(_ => content.MeasureHeight(Math.Max(1, Viewport.Width - 2)));
        content.Visible = expanded;
        content.SizeHintsChanged += NotifyLayoutChanged;

        Height = Dim.Func(_ => MeasureHeight(Math.Max(1, Viewport.Width)));

        Add(header, content);
    }

    public int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);

        if (!IsExpanded)
            return HeaderHeight;

        return HeaderHeight + Content.MeasureHeight(Math.Max(1, availableWidth - 2)) + ContentInset;
    }

    public void Toggle()
    {
        SetExpanded(!IsExpanded);
    }

    public void SetExpanded(bool expanded)
    {
        if (IsExpanded == expanded)
            return;

        if (!expanded && Content.HasFocus)
            toggleButton.SetFocus();

        IsExpanded = expanded;
        Content.Visible = expanded;
        toggleButton.Text = expanded ? "<" : ">";

        NotifyLayoutChanged();
    }

    private void NotifyLayoutChanged()
    {
        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }

    private static int TextWidth(string text) => TextFormatter.GetWidestLineLength(text, 4);
}
