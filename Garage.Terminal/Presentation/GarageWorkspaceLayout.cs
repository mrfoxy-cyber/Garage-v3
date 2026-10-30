using System.Collections.ObjectModel;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Presentation;

// Builds the dashboard controls. GarageWorkspace owns their events and behavior.
internal sealed class GarageWorkspaceLayout
{
    public Label Stats { get; }
    public Label Details { get; }
    public Label Status { get; }
    public FrameView TableFrame { get; }
    public FrameView DetailFrame { get; }
    public TableView Table { get; }
    public ListView Navigation { get; }
    public TextField Query { get; }

    public GarageWorkspaceLayout(Window workspace, WorkspaceButtons buttons)
    {
        workspace.Add(Theme.Apply(new Label
        {
            Text = "G / GARAGE", X = 2, Y = 1, Width = 20
        }, Theme.Accent));
        Stats = Theme.Apply(new Label
        {
            X = 23, Y = 2, Width = Dim.Fill(2), Height = 2
        }, Theme.Muted);
        buttons.AddHeaderTo(workspace);
        workspace.Add(Stats);

        FrameView sidebar = Theme.Apply(new FrameView
        {
            X = 1, Y = 4, Width = 21, Height = Dim.Fill(2),
            Title = "Workspace", BorderStyle = LineStyle.Rounded, CanFocus = true
        });
        Navigation = Theme.Apply(new ListView
        {
            X = 1, Y = 1, Width = Dim.Fill(1), Height = 6, CanFocus = true
        });
        Navigation.SetSource(new ObservableCollection<string>(
            ["Overview", "Vehicle catalog", "Parking", "Search garage"]));
        Navigation.SelectedItem = 0;
        Navigation.RowRender += (_, row) =>
            row.RowAttribute = row.Row == Navigation.SelectedItem
                ? Theme.Surface.Focus
                : Theme.Surface.Normal;
        sidebar.Add(Navigation, Theme.Apply(new Label
        {
            Text = "YOUR GARAGES", X = 1, Y = 8, Width = Dim.Fill(1), Height = 1
        }, Theme.Muted));
        buttons.AddSidebarTo(sidebar);
        workspace.Add(sidebar);

        View content = Theme.Apply(new View
        {
            X = 23, Y = 4, Width = Dim.Fill(1), Height = Dim.Fill(2), CanFocus = true
        });
        content.Add(Theme.Apply(new Label
        {
            Text = "Find", X = 0, Y = 0, Width = 5
        }, Theme.Muted));
        Query = Theme.Apply(new TextField
        {
            X = 6, Y = 0, Width = Dim.Fill(22), CanFocus = true
        }, Theme.Input);
        content.Add(Query);
        buttons.AddSearchTo(content);

        TableFrame = Theme.Apply(new FrameView
        {
            X = 0, Y = 2, Width = Dim.Fill(), Height = Dim.Fill(11),
            Title = "Overview", BorderStyle = LineStyle.Rounded, CanFocus = true
        });
        Table = Theme.Apply(new TableView
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(),
            FullRowSelect = true, MultiSelect = false, MaxCellWidth = 28, CanFocus = true,
            Style = new TableStyle
            {
                ShowHorizontalHeaderOverline = false,
                ShowHorizontalHeaderUnderline = true,
                ShowVerticalCellLines = false,
                ShowVerticalHeaderLines = false,
                ExpandLastColumn = true
            }
        });
        TableFrame.Add(Table);
        content.Add(TableFrame);

        DetailFrame = Theme.Apply(new FrameView
        {
            X = 0, Y = Pos.AnchorEnd(10), Width = Dim.Fill(), Height = 5,
            Title = "At a glance", BorderStyle = LineStyle.Rounded
        });
        Details = Theme.Apply(new Label
        {
            X = 1, Y = 0, Width = Dim.Fill(1), Height = Dim.Fill()
        }, Theme.Muted);
        DetailFrame.Add(Details);
        content.Add(DetailFrame);

        View actions = Theme.Apply(new View
        {
            X = 0, Y = Pos.AnchorEnd(4), Width = Dim.Fill(), Height = 4, CanFocus = true
        });
        buttons.AddActionsTo(actions);
        content.Add(actions);
        workspace.Add(content);

        Status = Theme.Apply(new Label
        {
            Text = "Ready. Select a row to see details.",
            X = 2, Y = Pos.AnchorEnd(1), Width = Dim.Fill(2)
        }, Theme.Accent);
        workspace.Add(Status);
    }

    public void ArrangeForHeight(int height)
    {
        bool compact = height > 0 && height < 30;
        if (DetailFrame.Visible == compact)
        {
            DetailFrame.Visible = !compact;
            TableFrame.Height = Dim.Fill(compact ? 5 : 11);
        }
    }
}
