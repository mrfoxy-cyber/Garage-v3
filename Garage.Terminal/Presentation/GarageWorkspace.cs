using Garage.Services;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Presentation;

internal sealed partial class GarageWorkspace : Window
{
    private readonly IApplication application;
    private readonly DashboardState state;
    private readonly Dialogs dialogs;
    private readonly Workflows workflows;
    private readonly WorkspaceButtons buttons;
    private readonly GarageWorkspaceLayout layout;
    private readonly IReadOnlyList<View> focusOrder;

    public GarageWorkspace(IApplication application, GarageService service)
    {
        this.application = application;
        state = new DashboardState(service);
        dialogs = new Dialogs(application);
        workflows = new Workflows(dialogs, service);
        buttons = new WorkspaceButtons(Execute);

        Title = "Garage Workspace";
        Width = Dim.Fill();
        Height = Dim.Fill();
        BorderStyle = LineStyle.None;
        Theme.Apply(this);

        layout = new GarageWorkspaceLayout(this, buttons);
        focusOrder = buttons.FocusOrder(layout.Navigation, layout.Query, layout.Table);
        ConnectEvents();
        layout.ArrangeForHeight(application.Screen.Height);
        RefreshRows();
    }

    private void ConnectEvents()
    {
        layout.Navigation.ValueChanged += (_, _) =>
        {
            state.Page = (WorkspacePage)(layout.Navigation.SelectedItem ?? 0);
            RefreshRows();
        };
        layout.Query.ValueChanged += (_, _) =>
        {
            state.QuickFilter = layout.Query.Text;
            RefreshRows();
        };
        layout.Table.ValueChanged += (_, _) =>
        {
            if (!refreshing) UpdateDetails();
        };
        layout.Table.Accepted += (_, e) =>
        {
            e.Handled = true;
            Execute(WorkspaceCommand.OpenDetails);
        };
        ViewportChanged += (_, _) => layout.ArrangeForHeight(Viewport.Height);
        KeyDown += (_, key) =>
        {
            if (key == Key.Tab || key == Key.Tab.WithShift)
            {
                AdvanceWorkspaceFocus(key == Key.Tab.WithShift);
                key.Handled = true;
            }
            else if (key == Key.F1) { Execute(WorkspaceCommand.Help); key.Handled = true; }
            else if (key == Key.F2) { Execute(WorkspaceCommand.CreateVehicle); key.Handled = true; }
            else if (key == Key.F3) { Execute(WorkspaceCommand.Search); key.Handled = true; }
            else if (key == Key.F4) { Execute(WorkspaceCommand.CreateGarage); key.Handled = true; }
            else if (key == Key.F5) { Execute(WorkspaceCommand.Refresh); key.Handled = true; }
            else if (key == Key.Q.WithCtrl) { Execute(WorkspaceCommand.Quit); key.Handled = true; }
            else if (key == Key.Esc) { Execute(WorkspaceCommand.Quit); key.Handled = true; }
            else if (key == Key.F.WithCtrl) { Execute(WorkspaceCommand.FocusFind); key.Handled = true; }
        };
    }

    // Buttons and keyboard shortcuts use the same actions.
    private void Execute(WorkspaceCommand command)
    {
        switch (command)
        {
            case WorkspaceCommand.ChooseGarage: Guard(ChooseGarage); break;
            case WorkspaceCommand.Quit: Quit(); break;
            case WorkspaceCommand.CreateGarage: Guard(() => GarageResult(workflows.CreateGarage())); break;
            case WorkspaceCommand.ImportGarage: Guard(() => GarageResult(workflows.ImportGarage())); break;
            case WorkspaceCommand.Help: Help(); break;
            case WorkspaceCommand.Search: Guard(Search); break;
            case WorkspaceCommand.CreateVehicle: Guard(() => CreateVehicle(false)); break;
            case WorkspaceCommand.ImportVehicle: Guard(ImportVehicle); break;
            case WorkspaceCommand.ParkVehicle: Guard(ParkVehicle); break;
            case WorkspaceCommand.RemoveVehicle: Guard(RemoveVehicle); break;
            case WorkspaceCommand.OpenDetails: Guard(OpenDetails); break;
            case WorkspaceCommand.ParkingActions: Guard(ParkingActions); break;
            case WorkspaceCommand.Refresh: RefreshRows(); break;
            case WorkspaceCommand.FocusFind: layout.Query.SetFocus(); break;
        }
    }

    // One tab sequence spans the dashboard panels.
    internal void AdvanceWorkspaceFocus(bool backwards = false)
    {
        List<View> available = focusOrder
            .Where(view => view.CanFocus && view.Visible && view.Enabled)
            .ToList();
        if (available.Count == 0) return;

        int index = MostFocused is View focused ? available.IndexOf(focused) : -1;
        int next = (index + (backwards ? -1 : 1) + available.Count) % available.Count;
        available[next].SetFocus();
    }
}
