# Garage V3

Garage V3 is a .NET 10 application for managing vehicles across multiple garages. It uses a Terminal.Gui v2 interface with forms, garage cards, searchable vehicle lists and collapsible panels.

You can create an empty garage, load a populated garage from JSON, park and remove vehicles, look up registrations, display vehicle-type counts, and search within one garage or across all garages.

## Projects

| Project | Responsibility |
| --- | --- |
| **Vehicle** | Vehicle models, capability interfaces, engines, specifications, factories and registration-format validation. |
| **Garage-V3** | The generic `Garage<T>`, `GarageHandler`, `IHandler`, search criteria, query records and operation results. |
| **Garage.Manager** | Multiple garages, garage IDs, global registration uniqueness, vehicle locations, searches across garages and JSON import. Exposes `IGarageManager`. |
| **Garage-V3.Console** | The Terminal.Gui v2 application. Its UI code uses the `Garage.Terminal` namespaces; the project folder is `v3.Console`. |
| **Garage.Tests** | Tests for garage operations, handler behaviour, manager workflows, vehicles and validation. |

Headless GUI tests exercise the terminal components in the UI test project.

## Build and run

Install the .NET 10 SDK, then open `Garage-V3.slnx` in Visual Studio and select the terminal application as the startup project.

From the solution directory:

```sh
dotnet build Garage-V3.slnx
dotnet run --project v3.Console
```

Run the tests with:

```sh
dotnet test Garage-V3.slnx
```

To run only the main test project:

```sh
dotnet test Garage.Tests/Garage.Tests.csproj
```

Use a resizable terminal window. A wider window can show more panels beside each other; smaller widths cause components to stack according to the adaptive layout.

## Using the application

The Home page provides garage creation, JSON loading and cards for existing garages. Open a garage to see its occupancy, parked vehicles and vehicle-type counts.

Parking collects the vehicle type, registration number and country of registration. Optional details include colour, wheel count, manufacturer, model and manufacturing year. A blank parking-spot field lets the garage choose the first free spot.

Removal can start from a particular garage or from the global menu. The global flow finds the vehicle and its garage before asking for confirmation. Search similarly supports a selected garage or all garages.

The bottom command bar provides the main actions:

| Key | Action |
| --- | --- |
| F1 | Home |
| F2 | Park |
| F3 | Remove |
| F4 | Search |
| F10 | Quit |

Panels can be collapsed to reduce clutter. Colours are managed by the theme, while reusable components handle forms, feedback, vehicle information and pixel artwork.

## Architecture

A typical user action follows this flow:

```text
Terminal views and components
            |
       UI services
            |
IGarageManager / GarageManager
            |
   IHandler / GarageHandler
            |
       Garage<IVehicle>
            |
    Vehicle models and interfaces
```

**`Garage<T>`** owns a private collection of parked vehicles and their spot numbers. It enforces capacity, registration uniqueness within the garage, valid spot numbers and spot availability. It implements `IEnumerable<T>`, so parked vehicles can be enumerated with `foreach`. External callers receive a read-only view of the parking entries.

**`GarageHandler`** coordinates operations for one garage. It provides vehicle lookup, search, garage information, parked-vehicle information and type counts. Requests return result objects containing success or failure information for the UI.

**`GarageManager`** owns the garages and their handlers. Each garage has an ID. The manager keeps track of vehicle locations and prevents a registration from being parked in more than one garage. It also supports finding and searching vehicles across garages.

**UI services** connect component events to manager operations and update the views. Views arrange components; components collect input or display information. Query records such as `GarageInfo` and `ParkedVehicleInfo` carry information between these layers.

## Vehicles, parking and search

The model includes cars, motorcycles, buses, airplanes, boats and trucks. Shared information belongs to the vehicle model, while optional specifications describe details such as dimensions, cabins and engines. Capability interfaces such as `IHasWheels` expose properties relevant to particular vehicle types.

Registration number and country are required for vehicle creation. Other information can be omitted. Consumers must handle missing specifications and nested details, such as an absent cabin.

Registration input is trimmed while preserving its letter casing. Lookup and duplicate detection ignore case. Format validation depends on the vehicle type and country; it validates supported formats rather than checking an official registration database.

A garage's capacity is measured in vehicle slots. Spot numbers start at **1**. Parking uses either a requested free spot or the first available spot. Vehicle dimensions remain part of the richer vehicle model; the current V3 garage uses capacity and numbered spaces for parking.

Search supports registration number, vehicle type, country, manufacturer, model, manufacturing year, colour and wheel count. Blank filters are ignored. Supplied filters are combined with **AND** logic, and text filters use whole-value comparisons that ignore case. Missing optional information does not match a filter requiring that information.

## Loading a garage from JSON

Use **Load garage** on the Home page. The current format stores one garage and its initial vehicles in a single file:

```json
{
  "Version": 1,
  "Name": "Example garage",
  "Capacity": 6,
  "Vehicles": [
    {
      "VehicleType": "Car",
      "RegistrationNumber": "ABC123",
      "CountryCode": "SWE",
      "Color": "Blue",
      "NumberOfWheels": 4,
      "Manufacturer": "Volvo",
      "Model": "V70",
      "ManufacturingYear": 2020
    },
    {
      "VehicleType": "Boat",
      "RegistrationNumber": "SABC",
      "CountryCode": "SWE"
    }
  ]
}
```

Use `"Vehicles": []` for an empty garage. Optional vehicle fields can be omitted, and initial vehicles are assigned sequential spots starting at 1.

Import constructs and validates the vehicles, checks capacity and duplicate registrations, and registers the garage through the manager. A failed import leaves existing garages unchanged. A successfully loaded garage also registers its vehicle locations for global lookup and duplicate detection.

Session changes remain in memory; they are not written back to the imported JSON file.

## Testing

Tests use Arrange–Act–Assert and descriptive names. Fixtures provide minimal or fully populated vehicles, garages with known parking assignments, and a manager containing mixed, single-vehicle-type and empty garages.

The test areas include:

- Garage construction, parking, removal, initialization and enumeration.
- Handler results, lookup, search, garage information and vehicle-type counts.
- Manager operations across garages, unique registrations, vehicle locations and failed operations preserving existing state.
- Vehicle validation and optional specifications.
- JSON loading and invalid input.
- Headless GUI component input, button events, removal confirmation and displayed type counts.

GUI logic can be tested without opening a terminal. Visual layout, resizing and animation should also be checked by running the application.

## Assignment scope

The core assignment functionality includes the constrained generic garage, enumeration, user-selected capacity, parking and removal, initial population, vehicle listing and type counts, case-insensitive lookup, combined search, a console interface, validation, interfaces and garage unit tests.

Multiple garages, global vehicle locations, JSON import, the additional truck type, adaptive panels and animated pixel artwork extend that core functionality.
