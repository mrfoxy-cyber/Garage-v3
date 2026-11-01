using Garage_V3.Interfaces;
using System.Reflection.Metadata;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Views;

public sealed class MainWindow : Runnable
{
    private readonly IHandler handler;

    private readonly TextField nameField;
    private readonly TextField capacityField;
    private readonly Label garageInfoLabel;
    private readonly Label feedbackLabel;

    public MainWindow(IHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        this.handler = handler;

        Title = "Garage V3";

        MenuBar menu = new()
        {
            Menus =
            [
                new MenuBarItem("_File", [new MenuItem("_Quit", "", () => App?.RequestStop())]),
                new MenuBarItem("_Help", [new MenuItem("_About", "", ShowAbout)])
            ]
        };

        StatusBar status = new();
        status.Add(new Shortcut(Key.F1, "About", ShowAbout));
        status.Add(new Shortcut(Application.GetDefaultKey(Command.Quit), "Quit", () => App?.RequestStop()));

        View content = new()
        {
            CanFocus = true,
            Y = Pos.Bottom(menu),
            Width = Dim.Fill(),
            Height = Dim.Fill(status)
        };

        garageInfoLabel = new()
        {
            Text = "No garage created.",
            X = 1,
            Y = 1,
            Width = Dim.Fill(1)
        };

        Label nameLabel = new()
        {
            Text = "Garage name:",
            X = 1,
            Y = Pos.Bottom(garageInfoLabel) + 2
        };

        nameField = new()
        {
            X = Pos.Right(nameLabel) + 2,
            Y = nameLabel.Y,
            Width = Dim.Percent(50)
        };

        Label capacityLabel = new()
        {
            Text = "Capacity:",
            X = 1,
            Y = Pos.Bottom(nameField) + 1
        };

        capacityField = new()
        {
            X = nameField.X,
            Y = capacityLabel.Y,
            Width = 12
        };

        Button createButton = new()
        {
            Text = "_Create garage",
            X = nameField.X,
            Y = Pos.Bottom(capacityField) + 1
        };

        createButton.Accepted += (_, _) => CreateGarage();

        feedbackLabel = new()
        {
            X = 1,
            Y = Pos.Bottom(createButton) + 1,
            Width = Dim.Fill(1),
            Height = Dim.Auto()
        };

        content.Add(garageInfoLabel, nameLabel, nameField, capacityLabel, capacityField, createButton, feedbackLabel);
        Add(menu, content, status);
    }

    private void CreateGarage()
    {
        if (!int.TryParse(capacityField.Text, out int capacity))
        {
            feedbackLabel.Text = "Enter a whole number for capacity.";
            capacityField.SetFocus();
            return;
        }

        var result = handler.CreateGarage(nameField.Text, capacity);

        feedbackLabel.Text = result.Message;

        if (result.Success)
        {
            nameField.Enabled = false;
            capacityField.Enabled = false;
            RefreshGarageInfo();
        }
    }

    private void RefreshGarageInfo()
    {
        var (info, result) = handler.GetGarageInfo();

        if (!result.Success || info is null)
        {
            feedbackLabel.Text = result.Message;
            return;
        }

        garageInfoLabel.Text = $"{info.GarageName} | Occupied: {info.OccupiedSpaces}/{info.Capacity} | Available: {info.AvailableSpaces}";
    }

    private void ShowAbout()
    {
        MessageBox.Query(App!, "About", "Garage V3\nBuilt with Terminal.Gui v2.", "OK");
    }
}
