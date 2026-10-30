# Implementation notes

These references describe the current application and its tests. They are separate from [what the assignment requires](../requirements/README.md). A source link or related passing test does not establish complete coverage of a use case.

## Create garage

- [GarageService](../../Garage/Services/GarageService.cs) currently exposes capacity, dimensions and JSON creation paths, with garage-name and registration checks.
- [Garage](../../Garage/Models/Garage.cs) stores a private list of parking spaces. Its current upper capacity limit of 100,000 is an implementation choice.
- [Dimensions](../../Vehicle/Vehicles/Components/Dimensions.cs) and [GarageJsonLoader](../../Garage/Persistence/Json/GarageJsonLoader.cs) support sized spaces, layouts and assignments. These extend the required baseline.
- [CreationResultTests](../../Garage.Tests/CreationResultTests.cs) cover successful garage creation, duplicate names and invalid capacity.
- [GarageConstructionTests](../../Garage.Tests/GarageConstructionTests.cs) cover initial vehicles and constructor rules.
- [GarageJsonLoaderTests](../../Garage.Tests/GarageJsonLoaderTests.cs) cover sized-space imports, duplicate custom numbers and fit rejection.
- [GarageServiceTests](../../Garage.Tests/GarageServiceTests.cs) cover duplicate garage names and failed imports preserving the garage collection.

Evidence still to add includes direct service tests for dimensions creation and for rejecting an imported registration without partial session changes. Those are enhancement checks, not newly inferred assignment requirements.

## Park vehicle

- [GarageService](../../Garage/Services/GarageService.cs) currently requires a catalogue vehicle and a registered garage, and checks whether the vehicle is already parked.
- [Garage](../../Garage/Models/Garage.cs) and [ParkingSpot](../../Garage/Models/ParkingSpot.cs) provide automatic or chosen-space assignment and dimension checks.
- [GarageParkingTests](../../Garage.Tests/GarageParkingTests.cs) cover parking, full garages, duplicates, chosen spaces, occupancy and fit checks.
- [GarageServiceWorkflowTests](../../Garage.Tests/GarageServiceWorkflowTests.cs) cover catalogue checks and moving a vehicle between garages after removal.
- [CreationResultTests](../../Garage.Tests/CreationResultTests.cs) cover returned parking results.

Direct service-level checks for invalid garage selection, out-of-range spaces and exact dimension equality remain separate coverage items. Existing lower-level or create-and-park tests are related evidence rather than proof of the entire parking workflow.

## Shared observations

Both current interfaces use the same domain rules. State is held in memory; input files are not updated when session data changes. The numbered console and richer terminal dashboard are different implementations of user interaction, not separate mandatory interfaces.

The use cases and this page are ordinary Markdown documents maintained by hand. Update these links when code or tests move. A use-case document or a related test link does not by itself establish complete assignment coverage.
