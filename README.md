# Garage

Garage is a .NET 10 application for creating vehicles and managing parking across multiple garages. You can create a garage with ordinary spaces or give its parking spots different dimensions, then create vehicles, park them, remove them, and search the collection. The same rules are available through two interfaces: a simple numbered-menu console and a richer terminal dashboard.

## Try it

From this directory, run either interface:

~~~sh
dotnet run --project Garage.Console
dotnet run --project Garage.Terminal
~~~

Both start with a sample **Lexicon Garage** and vehicles loaded from JSON. To explore the terminal dashboard without sample data, run `dotnet run --project Garage.Terminal -- --empty`. The terminal dashboard works best in a window at least 80 columns by 24 rows. See its [keyboard and screen guide](Garage.Terminal/README.md).

## How the application is organized

~~~text
Garage.Console ──┐
                 ├── Garage ── Vehicle
Garage.Terminal ─┘
~~~

| Project | Role |
| --- | --- |
| **Vehicle** | Defines vehicles, engines, specifications, factories, and registration-format validation. |
| **Garage** | Holds the garage and parking rules, the vehicle catalog, searches, operations, and JSON loading. |
| **Garage.Console** | Presents the application through numbered menus and input prompts. |
| **Garage.Terminal** | Presents the same application through a Terminal.Gui dashboard, forms, tables, and file browsing. |
| **Garage.Tests** | Checks the domain rules and application workflows. |

The **domain** is the part that decides what a valid vehicle or parking operation means. It lives in `Vehicle` and `Garage`; neither project depends on a user interface. Both interfaces call the same garage services, so parking a vehicle follows the same rules whichever screen you use.

The [Garage structure guide](Garage/README.md) explains how models, services, queries, results, and JSON loading connect, with a small example of the complete flow.

The [use-case documents](UseCases/README.md) describe required behavior from the Garage 1.0 assignment, with lists replacing its original array rule. They cover creation, parking, removal, listing, lookup, search, menus and exit; optional enhancements and implementation notes are kept separately. Read and edit the Markdown files directly through their index.

## Vehicles and the domain

Every vehicle has shared information such as a registration number, country of registration, manufacturer, model, year, and colour. Cars, motorcycles, buses, trucks, boats, and airplanes add their own specifications. A vehicle can also have engines, dimensions, and details specific to its type.

Vehicle factories create the finished objects. Registration validation checks supported formats for the vehicle type and country; it does not check whether a real-world registration has been issued. See [the validation rules](Vehicle/Vehicles/Validation.cs) for supported formats and their limits. Within the application, registration numbers must be unique regardless of letter case.

## Garages and parking

A garage has a name, a capacity, and numbered parking spots. Spots can be unrestricted or have a length, width, and height. For sized spots, a vehicle fits only when each of its dimensions is within the corresponding spot dimension; the vehicle is not rotated during this check.

The `Garage` model controls what occupies each spot. It can find a suitable free spot or try a specific spot, reject duplicates and vehicles that do not fit, remove vehicles, and search parked vehicles. Spot numbers shown to users start at **1**.

`GarageService` coordinates multiple garages and a shared vehicle catalog. The catalog includes parked vehicles and vehicles available to park. A vehicle can be parked in only one garage at a time. Removing it from a spot makes it available to park again. Operations return a result with a message, so the interfaces can show success or explain why an action failed.

## Choose an interface

**Garage.Console** is the straightforward, numbered-menu UI. From the main menu you can create or choose a garage and create or list vehicles. Inside a garage you can park at the first fitting spot or a chosen spot, inspect spots and parked vehicles, search by registration or other properties, and remove a vehicle. Vehicle and engine creation use guided prompts.

See the [console guide](Garage.Console/README.md) for menu instructions, JSON field reference tables, and complete import examples.

**Garage.Terminal** is a full-screen terminal UI. Its overview shows garages and occupancy; the vehicle catalog shows where each vehicle is parked. Parking and search have their own views, while forms guide vehicle and engine creation. It also provides a JSON file browser and keyboard shortcuts. The [terminal guide](Garage.Terminal/README.md) explains the screens and controls.

## How the terminal is built

The terminal app uses **Terminal.Gui** to draw windows, buttons, lists, tables, and dialogs inside the terminal. `Program.cs` loads the starting data, creates the shared `GarageService`, and starts the Terminal.Gui event loop with `GarageWorkspace` as its main window.

The workspace connects several small parts: a layout class creates the controls, a button class groups dashboard actions, and a state class tracks the selected garage, page, and filters. User actions call the shared garage services; rendering code then updates the visible table and details from the resulting state. The terminal project contains the screen behavior, while parking and vehicle rules stay in `Garage` and `Vehicle`.

See [how the terminal is built](Garage.Terminal/README.md#how-the-terminal-is-built) for the startup flow, file responsibilities, and where to make UI changes.

## JSON files

JSON lets you bring data into a running session. The sample files are in [Garage/Data](Garage/Data):

| File | Purpose |
| --- | --- |
| [example-vehicle.json](Garage/Data/example-vehicle.json) | One vehicle, including its specifications and engines. |
| [example-garage.json](Garage/Data/example-garage.json) | Garage name, capacity, default spot size, and optional custom spot sizes. |
| [example-placements.json](Garage/Data/example-placements.json) | Which registration number belongs in which spot; references a vehicle JSON file. |
| `vehicles.json` and `created-vehicles.json` | Arrays of vehicles used for the sample startup data. |

You can import a vehicle or create a garage from JSON through either interface. A garage layout and its parking assignments are **separate files**: you can import the layout alone for an empty garage, or add a placement file to fill specific spots. The placement file's `vehiclesFile` path is relative to the placement file. Imports check the vehicle data, spot numbers, duplicates, and whether assigned vehicles fit.

The bundled example placements refer to registrations already loaded at startup. To try that placement file, start with an empty session or use a vehicle file with different registrations. The terminal app offers `--empty`; the numbered-menu app starts with sample data.

Changes made in either interface stay in memory for that session. Importing JSON does not modify the source files, and quitting does not save the current garage state back to JSON.

## Build and test

~~~sh
dotnet build GarageArrays.slnx
dotnet test Garage.Tests/Garage.Tests.csproj
~~~

The tests cover vehicle validation, creation, parking and removal, searches, JSON loading, and duplicate handling.
