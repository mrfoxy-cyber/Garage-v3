using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Presentation;

// The dashboard's persistent buttons and their layout live together here.
// Dialog buttons stay with the dialogs that own them.
internal sealed class WorkspaceButtons
{
    public Button GaragePicker { get; }
    public Button Quit { get; }
    public Button CreateGarage { get; }
    public Button ImportGarage { get; }
    public Button Help { get; }
    public Button Filters { get; }
    public Button CreateVehicle { get; }
    public Button ImportVehicle { get; }
    public Button Park { get; }
    public Button Remove { get; }
    public Button Details { get; }
    public Button Spots { get; }

    public WorkspaceButtons(Action<WorkspaceCommand> execute)
    {
        // Header
        GaragePicker = Theme.Button("Choose garage", () => execute(WorkspaceCommand.ChooseGarage));
        GaragePicker.X = 23; GaragePicker.Y = 0; GaragePicker.Width = Dim.Fill(12);
        Quit = Theme.Button("_Quit", () => execute(WorkspaceCommand.Quit));
        Quit.X = Pos.AnchorEnd(10); Quit.Y = 0;

        // Sidebar
        CreateGarage = Theme.Button("+ _Garage", () => execute(WorkspaceCommand.CreateGarage));
        CreateGarage.X = 1; CreateGarage.Y = Pos.AnchorEnd(7); CreateGarage.Width = Dim.Fill(1);
        ImportGarage = Theme.Button("_Import garage", () => execute(WorkspaceCommand.ImportGarage));
        ImportGarage.X = 1; ImportGarage.Y = Pos.AnchorEnd(4); ImportGarage.Width = Dim.Fill(1);
        Help = Theme.Button("_Help / F1", () => execute(WorkspaceCommand.Help));
        Help.X = 1; Help.Y = Pos.AnchorEnd(2); Help.Width = Dim.Fill(1);

        // Search row
        Filters = Theme.Button("_Filters / F3", () => execute(WorkspaceCommand.Search));
        Filters.X = Pos.AnchorEnd(20); Filters.Y = 0;

        // Vehicle action bar
        CreateVehicle = Theme.Button("+ _Vehicle / F2", () => execute(WorkspaceCommand.CreateVehicle));
        CreateVehicle.X = 0; CreateVehicle.Y = 0;
        ImportVehicle = Theme.Button("_Import vehicle", () => execute(WorkspaceCommand.ImportVehicle));
        ImportVehicle.X = Pos.Right(CreateVehicle) + 1; ImportVehicle.Y = 0;
        Park = Theme.Button("_Park", () => execute(WorkspaceCommand.ParkVehicle));
        Park.X = Pos.Right(ImportVehicle) + 1; Park.Y = 0;
        Remove = Theme.Button("_Remove", () => execute(WorkspaceCommand.RemoveVehicle));
        Remove.X = 0; Remove.Y = 2;
        Details = Theme.Button("_Details", () => execute(WorkspaceCommand.OpenDetails));
        Details.X = Pos.Right(Remove) + 1; Details.Y = 2;
        Spots = Theme.Button("_Spots / create & park", () => execute(WorkspaceCommand.ParkingActions));
        Spots.X = Pos.Right(Details) + 1; Spots.Y = 2;
    }

    public void AddHeaderTo(View header) => header.Add(GaragePicker, Quit);
    public void AddSidebarTo(View sidebar) => sidebar.Add(CreateGarage, ImportGarage, Help);
    public void AddSearchTo(View content) => content.Add(Filters);
    public void AddActionsTo(View actions) => actions.Add(
        CreateVehicle, ImportVehicle, Park, Remove, Details, Spots);

    public IReadOnlyList<View> FocusOrder(ListView navigation, TextField query, TableView table) =>
    [
        GaragePicker, Quit, navigation, CreateGarage, ImportGarage, Help,
        query, Filters, table, CreateVehicle, ImportVehicle, Park, Remove, Details, Spots
    ];
}

internal enum WorkspaceCommand
{
    ChooseGarage,
    Quit,
    CreateGarage,
    ImportGarage,
    Help,
    Search,
    CreateVehicle,
    ImportVehicle,
    ParkVehicle,
    RemoveVehicle,
    OpenDetails,
    ParkingActions,
    Refresh,
    FocusFind
}
