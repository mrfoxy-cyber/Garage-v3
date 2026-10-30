# Garage Workspace

A separate Terminal.Gui app sharing the existing Garage and Vehicle projects. The original Garage.Console remains available.

## Run

From the Garage-v2 directory:

```text
dotnet run --project Garage.Terminal
```

To start without sample garages or vehicles:

```text
dotnet run --project Garage.Terminal -- --empty
```

Use a terminal with at least 80 columns and 24 rows; 120 by 32 or larger is recommended. The layout responds to resizing. Short terminals hide the details panel to show more table rows; the Details action still opens the full record. Colours adapt to terminal capabilities.

## Workspace

- **Overview:** garages, capacity, availability, and occupancy bars. Enter opens the selected garage.
- **Vehicle catalog:** parked and available vehicles from every garage, with current location and spot.
- **Parking:** vehicles in the selected garage. The garage selector stays at the top.
- **Search garage:** every existing search criterion, including multiple vehicle types and vehicle-specific filters. Find narrows the visible results as you type.

The sidebar creates garages or imports layout JSON with optional placements. Garage creation supports unrestricted spots, default dimensions, and individual spot overrides.

## Vehicle and engine creation

The vehicle wizard covers cars, motorcycles, airplanes, buses, boats, and trucks. Short form pages retain their values when moving back or correcting validation errors. Enum and boolean fields use selection lists. Numeric fields display their units and reject invalid values.

Before creating the vehicle, add, review, or remove engines. All ten existing engine types are supported. The same domain factories and Garage services perform creation and validation.

Use **Spots / create & park** to create a vehicle directly in the selected garage. Choose the first suitable spot or a specific open spot. Failed parking leaves the catalog unchanged.

## JSON files

Import actions open a file browser filtered to JSON, starting in the bundled Data folder. Browse files or paste a path into the location field.

- Vehicle import reads **one vehicle object** and can optionally park it immediately.
- Garage import reads a **layout**, followed optionally by a **placement file**.
- The formats and samples are the same as the original console.
- Successful vehicle imports select the new record in the catalog.

The [console guide's JSON section](../Garage.Console/README.md#import-json) contains complete examples and required-field tables for these shared formats.

## Keyboard and mouse

| Key | Action |
| --- | --- |
| Tab / Shift+Tab | Move between controls across all dashboard panels |
| Arrow keys | Move within lists and tables |
| Enter | Activate a button, open a choice, or inspect a table row |
| Space | Mark a vehicle type in the multi-selection list |
| Ctrl+F | Focus the live Find field |
| F1 | Help |
| F2 | Create a vehicle |
| F3 | Search filters |
| F4 | Create a garage |
| F5 | Refresh |
| Esc | Cancel a dialog; on the dashboard, ask to quit |
| Ctrl+Q | Ask to quit |

Mouse selection and buttons are also supported. Removal asks for confirmation and keeps the vehicle in the catalog as available.

## Session data

Like the original console, changes stay in the running session. Imports never overwrite their source files. Quitting does not save garages or vehicles back to JSON.

## How the terminal is built

The app targets **.NET 10** and uses **Terminal.Gui 2.4.17**. Terminal.Gui supplies the terminal controls, keyboard and mouse events, focus handling, and application loop. The main dashboard is a `Window` subclass called `GarageWorkspace`.

### Startup

1. [Program.cs](Program.cs) reads the startup options and creates `GarageService`, either with sample data or with an empty collection.
2. It creates and initializes the Terminal.Gui application.
3. `GarageWorkspace` creates its UI state, dialogs, workflows, buttons, and layout.
4. The workspace connects control events, adapts the layout to the terminal height, and displays the initial data.
5. `app.Run(workspace)` processes input until the user confirms quitting. The application and window are disposed when the run ends.

### Layout, behavior, and display

The files in [Presentation](Presentation) each have one responsibility:

| File | Responsibility |
| --- | --- |
| [GarageWorkspace.cs](Presentation/GarageWorkspace.cs) | Connect state, layout, events, keyboard shortcuts, and commands. Start here when reading the dashboard code. |
| [GarageWorkspaceLayout.cs](Presentation/GarageWorkspaceLayout.cs) | Create and place controls, expose their named references, and adapt to terminal height. |
| [GarageWorkspace.Rendering.cs](Presentation/GarageWorkspace.Rendering.cs) | Refresh the table, statistics, details, and button availability. |
| [GarageWorkspace.Actions.cs](Presentation/GarageWorkspace.Actions.cs) | Handle user actions such as creating, importing, parking, and searching. |
| [WorkspaceButtons.cs](Presentation/WorkspaceButtons.cs) | Define persistent dashboard buttons, their placement, and their commands. |
| [DashboardState.cs](Presentation/DashboardState.cs) | Track the selected garage, page, quick filter, and search criteria; select the vehicles to display. |
| [Workflows.cs](Presentation/Workflows.cs) | Guide multi-step vehicle, engine, and garage creation. |
| [Dialogs.cs](Presentation/Dialogs.cs) | Build temporary forms, choices, confirmations, messages, and file selection dialogs. |
| [VehicleDetails.cs](Presentation/VehicleDetails.cs) | Format vehicle and engine information for display. |
| [Theme.cs](Presentation/Theme.cs) | Define colours and shared control styling. |

The three `GarageWorkspace` files marked `partial` form **one C# class**. The file split lets you read startup and input handling separately from user actions and rendering; they share the same state and layout instance.

`GarageWorkspaceLayout` builds the header, sidebar, search row, vehicle table, details panel, action bar, and status line. Controls use `Pos` for placement and `Dim` for sizing, including filling the remaining space and anchoring to an edge. The workspace passes height changes to `ArrangeForHeight`, which hides the details panel in short terminals so the table has more room.

### From a button click to a result

Buttons and keyboard shortcuts send the same `WorkspaceCommand` to the workspace's `Execute` method. That method selects the action to run. For example, parking follows this flow:

```text
Park button
  -> Execute(ParkVehicle)
  -> choose a vehicle and a suitable spot
  -> GarageService.Park(...)
  -> show the result message
  -> RefreshRows()
```

`GarageService` coordinates the catalog and garages; the garage model checks occupancy, registration uniqueness, and vehicle dimensions. The UI displays the returned result. `RefreshRows` reads the current state again, rebuilds the visible table, updates the details, and enables the appropriate buttons.

The [Garage structure guide](../Garage/README.md) explains how those shared models, services, queries, and results connect.

Sidebar navigation and the Find field change `DashboardState` and refresh the view. They do not change which vehicles are parked. Expected operation failures appear as result messages, while unexpected dashboard action errors are logged and shown in a dialog.

### Forms and factories

[Forms](Forms) contains the input descriptions and adapters used by the dialogs:

- [FieldDefinition.cs](Forms/FieldDefinition.cs) describes a field's name, label, type, required status, and initial value. It parses entered text into values such as numbers, booleans, and enum choices.
- [FactoryDefinition.cs](Forms/FactoryDefinition.cs) lists the supported vehicle and engine forms.
- [FactoryDefinition.Fields.cs](Forms/FactoryDefinition.Fields.cs) lists the fields needed by each factory.
- [FactoryDefinition.Create.cs](Forms/FactoryDefinition.Create.cs) maps the completed form values to explicit domain factory calls.
- [SearchForm.cs](Forms/SearchForm.cs) turns optional search inputs into vehicle search criteria.

Forms can hold unfinished input and retain it while the user corrects an error. A completed vehicle is passed through `VehicleCreationService` before it joins the application catalog. Domain validation remains in the shared `Garage` and `Vehicle` projects. `Garage.Terminal` references `Garage`, which references `Vehicle`; it does not reference `Garage.Console`.

### Where to make a change

| Change | Start with |
| --- | --- |
| Move or resize a dashboard panel | `GarageWorkspaceLayout.cs` |
| Change a dashboard button's label or placement | `WorkspaceButtons.cs` |
| Add a dashboard action or keyboard shortcut | `WorkspaceCommand` and `GarageWorkspace.Execute` / `ConnectEvents` |
| Change table columns or summaries | `GarageWorkspace.Rendering.cs` and `VehicleDetails.cs` |
| Change a creation form | `FactoryDefinition.Fields.cs` and its matching call in `FactoryDefinition.Create.cs` |
| Change colours | `Theme.cs` |
| Change parking or vehicle rules | The shared `Garage` or `Vehicle` project |

Build the terminal project with the .NET 10 SDK and run the solution's tests from the repository root:

```text
dotnet build Garage.Terminal/Garage.Terminal.csproj
dotnet test Garage.Tests/Garage.Tests.csproj
```

For Terminal.Gui API details, see the [Terminal.Gui documentation](https://tui-cs.github.io/Terminal.Gui/docs/getting-started.html).
