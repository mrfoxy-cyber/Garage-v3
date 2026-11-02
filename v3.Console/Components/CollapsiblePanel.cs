using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using Garage.Terminal.Theming;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class CollapsiblePanel<T> : View, IAdaptiveComponent where T : View, IAdaptiveComponent
{
    private const int HeaderHeight = 1;
    private const int ContentInset = 1;

    private readonly string title;
    private readonly Button toggleButton;

    public T Content { get; }
    public bool IsExpanded { get; private set; }

    public event Action? SizeHintsChanged;

    private int ToggleWidth => TextWidth(toggleButton.TextFormatter.Text)
        + toggleButton.GetAdornmentsThickness().Horizontal;

    public int MinimumWidth => Math.Max(
        ToggleWidth + 2,
        IsExpanded ? Content.MinimumWidth + ContentInset * 2 : 0);

    public int PreferredWidth => Math.Max(
        TextWidth(title) + ToggleWidth + 2,
        IsExpanded ? Content.PreferredWidth + ContentInset * 2 : MinimumWidth);

    public CollapsiblePanel(string title, T content, GarageTheme theme, bool expanded = true)
    {
        this.title = title;
        Content = content;
        IsExpanded = expanded;

        CanFocus = true;
        SchemeName = theme.GroupScheme;

        View header = new()
        {
            CanFocus = true,
            Width = Dim.Fill(),
            Height = HeaderHeight,
            SchemeName = theme.PanelHeaderScheme
        };

        toggleButton = new()
        {
            Text = expanded ? "-" : "+",
            X = Pos.AnchorEnd(),
            Width = Dim.Auto(),
            Height = Dim.Auto()
        };

        theme.StyleChoice(toggleButton);
        toggleButton.SchemeName = theme.PanelHeaderScheme;

        toggleButton.TextFormatter.WordWrap = false;
        toggleButton.Accepted += (_, _) => Toggle();

        Label titleLabel = new()
        {
            Text = title,
            X = ContentInset,
            Width = Dim.Fill(toggleButton) - 1,
            Height = HeaderHeight,
            SchemeName = theme.PanelHeaderScheme
        };

        titleLabel.TextFormatter.WordWrap = false;

        header.Add(titleLabel, toggleButton);

        content.X = ContentInset;
        content.Y = HeaderHeight;
        content.Width = Dim.Fill(ContentInset);
        content.Height = Dim.Func(_ => content.MeasureHeight(
            Math.Max(1, Viewport.Width - ContentInset * 2)));
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

        int contentWidth = Math.Max(1, availableWidth - ContentInset * 2);

        return HeaderHeight + Content.MeasureHeight(contentWidth) + ContentInset;
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
        toggleButton.Text = expanded ? "-" : "+";

        NotifyLayoutChanged();
    }

    private void NotifyLayoutChanged()
    {
        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Content.SizeHintsChanged -= NotifyLayoutChanged;

        base.Dispose(disposing);
    }

    private static int TextWidth(string text) => TextFormatter.GetWidestLineLength(text, 4);
}
