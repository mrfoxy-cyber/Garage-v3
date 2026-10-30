# Garage: shared application logic

This project is the shared core used by **Garage.Console** and **Garage.Terminal**. It keeps track of garages and the vehicle catalog, applies parking rules, answers searches, and loads JSON data. The separate **Vehicle** project defines vehicles, engines, specifications, and their factories.

The main idea is that a screen asks for information or requests an operation. This project answers using data or an operation result. It does not choose menus, read keyboard input, or print messages to the screen.

## The structure at a glance

| Folder | Main responsibility | Example |
| --- | --- | --- |
| [Models](Models) | Hold the live state and protect its rules. | A garage owns its spots and their occupants. |
| [Services](Services) | Coordinate operations involving the catalog and multiple garages. | Prevent a vehicle from being parked in two garages. |
| [Queries](Queries) | Describe search inputs and readable views of garage information. | Find matching vehicles or return a vehicle with its spot number. |
| [Results](Results) | Describe the outcome of an operation. | Parking succeeded at spot 2, or failed because the spot was occupied. |
| [Specifications](Specifications) | Describe imported garage layouts and parking assignments. | Default spot dimensions and custom spot sizes. |
| [Persistence/Json](Persistence/Json) | Read JSON, map its data, and construct valid models. | Load one vehicle or a garage with assigned occupants. |
| [Diagnostics](Diagnostics) | Record unexpected errors. | Write an exception and its context to a log. |
| [Data](Data) | Provide sample files. | Vehicle catalogs and garage import examples. |

## Models: where the live state belongs

[Garage](Models/Garage.cs) represents one garage. It owns a private list of [ParkingSpot](Models/ParkingSpot.cs) objects and provides methods to park, remove, and search vehicles. A spot stores its number, optional dimensions, and occupant. The spot class is internal, so an interface cannot directly replace an occupant.

A garage enforces rules within its own boundary: capacity, valid spot numbers, empty spots, duplicate registrations, and whether a vehicle fits. Sized spots compare length, width, and height directly. An unrestricted spot has no dimension limit. Public spot numbers start at **1**.

[VehicleCatalog](Models/VehicleCatalog.cs) contains every vehicle registered in the running application, including both parked vehicles and vehicles available to park. Parking changes a spot's occupant; it does not move the vehicle out of the catalog. Removing a vehicle from a garage clears its spot and leaves it available in the catalog.

Public collections provide read-only access or views of the model's collections. This protects collection membership and parking state; it does not create a deep copy of every vehicle object.

## Services: coordinate a complete operation

[GarageService](Services/GarageService.cs) owns the collection of registered garages and the shared catalog. It checks rules that span those collections, such as unique garage names and vehicle registrations. Its `Park` method checks catalog membership and whether the vehicle is already parked anywhere, then asks the selected garage to perform its own parking checks.

For parking from an application screen, use `GarageService.Park`. Calling a single garage's `Park` directly checks that garage only; it cannot know about occupants in other garages. Removal is performed through the selected garage's `Remove` or `RemoveAtSpot` method.

The service also exposes [VehicleCreationService](Services/VehicleCreationService.cs) through `VehicleCreation`. It creates or imports a vehicle, checks registration uniqueness, and optionally parks it before adding it to the catalog. If creation or the requested parking fails, the new vehicle is not registered. This gives **create and park** one consistent outcome.

## Queries and results: how they connect

The distinction is about the question being answered:

| Request | Kind | Answer |
| --- | --- | --- |
| Which vehicles can fit here? | Query | A read-only list of vehicles. |
| Where is registration ABC123 parked? | Query | A `ParkedVehicleInfo`, or `null` when absent. |
| Park this vehicle in spot 2. | Operation | A `ParkingResult` explaining success or failure. |
| Remove this vehicle. | Operation | A `GarageOperationResult` describing the removal. |

A typical screen uses both:

```text
Read available vehicles and spots
  -> user makes a selection
  -> request parking through GarageService
  -> inspect ParkingResult and show its message
  -> read garage information again to refresh the screen
```

The `Queries` folder is a group of search and display types. It does not represent a database or a separate copy of the garage. Queries read the current models, and operation results report what happened when those models were asked to change.

### Query inputs and readable records

| Type or method | Purpose |
| --- | --- |
| [VehicleSearchCriteria](Queries/VehicleSearchCriteria.cs) | Optional filters passed to `Garage.Search`. A null filter is ignored; an empty vehicle-type list means any type. |
| [VehicleSearch](Queries/VehicleSearch.cs) | Internal matching rules used by `Garage.Search`. It handles shared and vehicle-specific filters. |
| [ParkingSpotInfo](Queries/ParkingSpotInfo.cs) | Spot number, dimensions, and occupancy returned by `Garage.SpotDetails`. |
| [ParkedVehicleInfo](Queries/ParkedVehicleInfo.cs) | A vehicle together with its spot number, returned by `ParkedVehicles` and `FindVehicleByRegistration`. |
| [VehicleTypeGroup](Queries/VehicleTypeGroup.cs) | A vehicle-type name and its parked vehicles, returned by `VehiclesByType`. The count comes from the group's vehicle list. |
| `Garage.AvailableSpotsFor(vehicle)` | Suitable free spot numbers within one garage. |
| `GarageService.GetParkableVehicles(garage, spotNumber)` | Catalog vehicles that are not parked anywhere and fit the target garage or selected spot. |

Selected search filters must all match. Selecting multiple vehicle types allows a match against any of those types. Search results contain vehicles parked in the searched garage; an empty list means no matches. Searching does not change parking state.

### Operation results

The classes in [Results](Results) give both interfaces the same way to handle expected outcomes:

| Result | Returned by | Success data |
| --- | --- | --- |
| [GarageCreationResult](Results/GarageCreationResult.cs) | Garage creation and import through `GarageService`. | The created `Garage`. |
| [VehicleCreationResult](Results/VehicleCreationResult.cs) | `VehicleCreation.Create` and `FromJson`. | The created `Vehicle`, with optional parking details. |
| [ParkingResult](Results/ParkingResult.cs) | Parking through a garage or `GarageService`. | The assigned spot number. |
| [GarageOperationResult](Results/GarageOperationResult.cs) | `Garage.Remove` and `RemoveAtSpot`. | The cleared spot number. |

Each result has `Success`, `Message`, and an [OperationError](Results/OperationError.cs) code. `Success` is true when the error is `None`. Examples of error codes are `GarageFull`, `SpotOccupied`, `VehicleDoesNotFit`, and `DuplicateRegistration`.

The UI can display `Message` and use `Error` for decisions, without interpreting message text. Check `Success` before using a created object or spot number. Failed creation contains no created object; failed parking contains no assigned spot. `VehicleCreationResult` can also represent cancellation through `Cancelled` and the `Cancelled` error code.

Results use private constructors and named methods such as `Completed`, `Failed`, and `Cancel`. This keeps outcomes consistent: a successful creation must contain its object, and a failed operation must have an error code.

## A small example

The following example creates an empty application, imports a vehicle, queries suitable spots, and parks it. Run with the repository root as the working directory so the sample path resolves:

```csharp
using Garage.Queries;
using Garage.Results;
using Garage.Services;
using Garage.Vehicles;
using VehicleGarage = Garage.Models.Garage;

GarageService service = new([], []);

GarageCreationResult creation = service.CreateGarage("Workshop", 3);
if (!creation.Success || creation.Garage is not VehicleGarage garage)
{
    Console.WriteLine(creation.Message);
    return;
}

VehicleCreationResult import = service.VehicleCreation.FromJson(
    "Garage/Data/example-vehicle.json");
if (!import.Success || import.Vehicle is not Vehicle vehicle)
{
    Console.WriteLine(import.Message);
    return;
}

IReadOnlyList<int> availableSpots = garage.AvailableSpotsFor(vehicle);
ParkingResult parking = service.Park(vehicle, garage);
Console.WriteLine(parking.Message);

VehicleSearchCriteria criteria = new() { Manufacturer = "Volvo" };
IReadOnlyList<Vehicle> matches = garage.Search(criteria);
```

`availableSpots` and `matches` answer queries. `creation`, `import`, and `parking` describe operation outcomes. A screen can use the same calls and decide how to present the data and messages.

## JSON specifications and model creation

The JSON flow starts with file data and ends with live models:

```text
JSON file
  -> loader and input contract/specification
  -> mapper or garage construction
  -> vehicle factories and model validation
  -> service registration
  -> creation result returned to the UI
```

[GarageSpecification](Specifications/GarageSpecification.cs) describes a layout, including capacity and optional spot dimensions. [GaragePlacementSpecification](Specifications/GaragePlacementSpecification.cs) describes occupants by registration and spot number. These are separate descriptions so a layout can be imported with or without parked vehicles.

[VehicleJsonLoader](Persistence/Json/VehicleJsonLoader.cs) reads a single vehicle object or a catalog of vehicle objects. Its internal contracts describe the file shape, and [VehicleJsonMapper](Persistence/Json/VehicleJsonMapper.cs) calls the domain factories. [GarageJsonLoader](Persistence/Json/GarageJsonLoader.cs) builds a garage and applies validated assignments.

Loaders construct objects but do not register them with an existing application service. UI imports therefore use `GarageService.CreateGarageFromJson` or `VehicleCreation.FromJson`, which also enforce the running application's uniqueness rules. See the [console JSON guide](../Garage.Console/README.md#import-json) for complete file examples and required fields. Current persistence support reads JSON; it does not save session changes back to files.

## Validation, results, and exceptions

An unavailable parking spot is an expected operation outcome, so parking returns a failed result. Invalid model construction or invalid arguments can throw exceptions because the requested object or call is not valid. The creation services use [ExpectedFailure](Services/ExpectedFailure.cs) to turn expected validation and file exceptions into creation results, including codes such as `InvalidJson` and `FileNotFound`.

Unexpected exceptions can reach the interface's error handling, which calls [ErrorLogger](Diagnostics/ErrorLogger.cs). It attempts to write diagnostic context to `Logs/errors.log` beneath the application's output directory. Logging itself does not throw another error if the log cannot be written.

## Where to add or change behavior

| Change | Start with |
| --- | --- |
| A rule for one parking spot or garage | `ParkingSpot` or `Garage` in Models. |
| A rule involving multiple garages or the shared catalog | `GarageService`. |
| Vehicle creation, duplicate checks, or create-and-park behavior | `VehicleCreationService`. |
| A search filter | `VehicleSearchCriteria` and `VehicleSearch`, then the input controls in each interface. |
| Information shown in a parking query | The appropriate query record and the model property that produces it. |
| A new expected failure reason | `OperationError` and the operation returning that result. |
| A new JSON field | The relevant contract/specification and mapper, then its documentation and validation. |
| Vehicle or engine properties and validation | The separate Vehicle project. |

The tests in [Garage.Tests](../Garage.Tests) cover these connections: model parking rules, service workflows, searches, result invariants, and JSON imports. Run them from the repository root with `dotnet test Garage.Tests/Garage.Tests.csproj`.
