using Garage.Terminal.Interfaces;
using Garage.Terminal.Layout;
using System.Drawing;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Components;

public sealed class GarageInput : View, IAdaptiveComponent
{
    // Usability preferences, not fixed screen dimensions.
    private const int MinimumInputWidth = 12;
    private const int PreferredInputWidth = 30;
    private const int Gap = 1;

    private readonly Label nameLabel;
    private readonly Label capacityLabel;
    private readonly TextField nameField;
    private readonly TextField capacityField;
    private readonly Button createButton;
    private readonly Label errorLabel;

    private readonly (View Control, int GapBefore)[] controls;

    public event Action<string, int>? CreateRequested;
    public event Action? SizeHintsChanged;

    public int MinimumWidth => Math.Max(MinimumInputWidth,
        Math.Max(NaturalWidth(nameLabel), Math.Max(NaturalWidth(capacityLabel), NaturalWidth(createButton))));

    public int PreferredWidth => Math.Max(MinimumWidth, PreferredInputWidth);

    public GarageInput()
    {
        CanFocus = true;

        nameLabel = new() { Text = "Garage name:" };
        nameField = new();

        capacityLabel = new() { Text = "Capacity:" };
        capacityField = new();

        createButton = new() { Text = "_Create garage" };
        errorLabel = new() { Visible = false };

        controls =
        [
            (nameLabel, 0),
            (nameField, 0),
            (capacityLabel, Gap),
            (capacityField, 0),
            (createButton, Gap),
            (errorLabel, Gap)
        ];

        View? previous = null;

        foreach (var item in controls)
        {
            item.Control.X = 0;
            item.Control.Y = previous is null ? 0 : Pos.Bottom(previous) + item.GapBefore;
            item.Control.Width = Dim.Fill();

            if (item.Control is Label)
                item.Control.Height = Dim.Auto(DimAutoStyle.Text);

            Add(item.Control);
            previous = item.Control;
        }

        Height = Dim.Func(_ => MeasureHeight(Math.Max(1, Viewport.Width)));

        createButton.Accepted += (_, _) => RequestCreation();
    }

    public int MeasureHeight(int availableWidth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(availableWidth);

        int height = 0;

        foreach (var item in controls)
        {
            if (!item.Control.Visible)
                continue;

            height += item.GapBefore + MeasureControlHeight(item.Control, availableWidth);
        }

        return height;
    }

    public void ShowError(string message)
    {
        errorLabel.Text = message;
        errorLabel.Visible = !string.IsNullOrWhiteSpace(message);

        SetNeedsLayout();
        SizeHintsChanged?.Invoke();
    }

    private void RequestCreation()
    {
        ShowError(string.Empty);

        if (!int.TryParse(capacityField.Text, out int capacity))
        {
            ShowError("Enter a whole number for capacity.");
            capacityField.SetFocus();
            return;
        }

        CreateRequested?.Invoke(nameField.Text.Trim(), capacity);
    }

    private static int NaturalWidth(View control)
    {
        int textWidth = TextFormatter.GetWidestLineLength(control.Text, 4);
        return textWidth + control.GetAdornmentsThickness().Horizontal;
    }

    private static int MeasureControlHeight(View control, int availableWidth)
    {
        var adornments = control.GetAdornmentsThickness();

        if (control is TextField)
            return 1 + adornments.Vertical;

        TextFormatter formatter = new()
        {
            Text = control.Text,
            WordWrap = control is Label,
            MultiLine = true,
            HotKeySpecifier = control.TextFormatter.HotKeySpecifier
        };

        int contentWidth = Math.Max(1, availableWidth - adornments.Horizontal);
        var size = formatter.FormatAndGetSize(new Size(contentWidth, int.MaxValue));

        return Math.Max(1, size.Height) + adornments.Vertical;
    }
}
