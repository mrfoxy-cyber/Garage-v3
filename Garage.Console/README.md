# Garage Console

The numbered-menu interface for creating vehicles and managing multiple garages. It uses the same vehicle models, parking rules, and JSON loaders as Garage.Terminal.

## Start the application

With the .NET 10 SDK installed, run this from the repository root:

```sh
dotnet run --project Garage.Console
```

In Visual Studio, open `GarageArrays.slnx`, set **Garage.Console** as the startup project, and run it. The application starts with **Lexicon Garage**, capacity 100, and sample vehicles from [Garage/Data](../Garage/Data). This interface currently always loads that startup data.

## Navigate the menus

Type an option's number and press **Enter**. Choose **0** to go back; **0** in the main menu exits. After an operation, read its result and press Enter when prompted to continue. Invalid menu input prompts you again.

| Main menu option | What it does |
| --- | --- |
| Create and list vehicles | Create a car, motorcycle, airplane, bus, boat, or truck; import one vehicle from JSON; list the catalog. |
| Choose garage | Open an existing garage. Its name and occupied/total spots appear in the selection list. |
| Create garage (unrestricted spots) | Enter a unique name and capacity; its spots have no dimension limits. |
| Create garage with sized parking spots | Enter a default spot size, then optionally override individual spots. |
| Create garage from JSON files | Load a garage layout, optionally with a separate parking assignments file. |

### Create and park a vehicle

1. Choose **Create and list vehicles**, then a vehicle type.
2. Open each field you want to fill. Menu labels show the current draft values. Fill the common details, dimensions, and fields specific to that vehicle type. Dimensions are in meters and weight is in kilograms.
3. Add engines if needed. You can list or remove draft engines before creating the vehicle.
4. Choose **Create vehicle**. The application reports missing or invalid information. **Clear all fields** resets that draft.
5. Choose **0** to return, open a garage through **Choose garage**, and choose **Park a vehicle**.
6. Select an available vehicle. The list includes only vehicles that fit an open spot. The application chooses the first suitable spot and reports its number.

To create and park in one flow, open a garage first and choose **Create and park vehicle**. **Create and park vehicle at a specific spot** asks for the spot before opening the creator. A failed creation or parking attempt does not add the new vehicle to the catalog.

### Work inside a garage

| Option | Use |
| --- | --- |
| Park a vehicle to a specific spot | Choose an open spot, then an available vehicle that fits it. |
| List parking spots and sizes | Inspect spot numbers, size limits, and occupancy. |
| List all parked vehicles | See each parked vehicle and its spot. |
| List vehicle types | See the parked vehicles grouped by type, with counts. |
| Find vehicle by registration number | Find a parked vehicle; letter case is ignored. |
| Search vehicles | Set common or type-specific filters, then run the search. Unset filters mean Any. Multiple filters must all match. |
| Remove vehicle | Remove a vehicle from this garage by registration number. It remains in the catalog and can be parked again. |

Spot numbers start at **1**. A vehicle can occupy only one garage at a time. For sized spots, length, width, and height are compared directly; the fit check does not rotate the vehicle.

## Import JSON

Save the JSON to a file before importing it. At a path prompt, enter the path as plain text, without surrounding quotation marks. Absolute paths work from any launch location. Relative paths entered in the menus use the process's current working directory; when launched from the repository root, for example:

```text
Garage/Data/example-vehicle.json
```

| Import | Required JSON root |
| --- | --- |
| Create vehicle from JSON file | One vehicle object: `{ ... }` |
| Garage configuration | One layout object: `{ ... }` |
| Parking assignments | One placement object: `{ ... }` |
| The vehicle catalog referenced by a placement file | A JSON array of vehicle objects: `[ ... ]` |

JSON property names are case-insensitive, but unknown properties are rejected. Use quoted enum names such as `"SWE"`, `"Black"`, and `"Gasoline"`; numeric enum values are rejected. JSON numbers use a decimal point, and booleans are `true` or `false`. Comments and trailing commas are not supported.

### Example: import one car

Save this as `my-car.json`:

```json
{
  "type": "Car",
  "registrationNumber": "NEW123",
  "countryCode": "SWE",
  "manufacturer": "Volvo",
  "model": "V70",
  "manufacturingYear": 2018,
  "color": "Black",
  "weightKg": 1700,
  "lengthMeters": 4.8,
  "widthMeters": 1.8,
  "heightMeters": 1.5,
  "vin": "VOLVO-V70-001",
  "bodyType": "Sedan",
  "numberOfDoors": 5,
  "numberOfSeats": 5,
  "engines": [
    {
      "type": "Inline",
      "serialNumber": "ENG-VOLVO-001",
      "model": "B4204T",
      "fuelType": "Gasoline",
      "horsePower": 190,
      "displacementCc": 1969,
      "cylinderCount": 4
    }
  ]
}
```

Choose **Create and list vehicles → Create vehicle from JSON file** and enter its path. This adds the car to the catalog. If you open the import option through a garage's **Create and park vehicle** flow, the imported vehicle is also parked there.

The same example is bundled as [example-vehicle.json](../Garage/Data/example-vehicle.json). A registration already in the catalog cannot be imported again.

### Vehicle and engine fields

Every vehicle needs `type`, `registrationNumber`, `countryCode`, `manufacturer`, `model`, `manufacturingYear`, `color`, `weightKg`, `lengthMeters`, `widthMeters`, and `heightMeters`. Use a supported country code and registration format. Dimensions must be positive finite numbers. `engines` is a JSON array of engine objects; use `[]` for no engines.

The fields below are additionally required for each vehicle type, even when a count is zero or a boolean is false:

| Vehicle `type` | Additional fields |
| --- | --- |
| Car | `vin`, `bodyType`, `numberOfDoors`, `numberOfSeats` |
| Motorcycle | `vin`, `motorcycleType`, `hasSidecar`, `numberOfWheels`, `passengerCapacity`, `driverSeats` |
| Bus | `vin`, `numberOfDoors`, `numberOfWheels`, `passengerSeatCount`, `standingPassengerCapacity`, `driverSeatCount` |
| Truck | `vin`, `numberOfWheels`, `numberOfAxles`, `maximumLoadKg` |
| Boat | `hin`, `draftMeters`, `boatType`, `passengerCapacity`, `driverSeats`, `hasCabin`, `cabinPassengerSeatCount`, `cabinCrewSeatCount` |
| Airplane | `airframeSerialNumber`, `numberOfWheels`, `wingspanMeters`, `passengerSeatCount`, `crewSeatCount`, `pilotSeatCount`, `hasDualControls`, `maximumAltitudeMeters` |

Each engine needs `type`, `serialNumber`, and `model`, plus these fields:

| Engine `type` | Additional fields |
| --- | --- |
| Inline, VEngine, Flat | `fuelType`, `horsePower`, `displacementCc`, `cylinderCount` |
| Rotary | `fuelType`, `horsePower`, `displacementCc`, `rotorCount` |
| ElectricMotor | `powerKw`, `voltage` |
| Steam | `fuelType`, `horsePower`, `boilerPressureBar` |
| Turbojet | `fuelType`, `thrustKn` |
| Turbofan | `fuelType`, `thrustKn`, `bypassRatio` |
| Turboprop, Turboshaft | `fuelType`, `thrustKn`, `shaftHorsePower` |

See [vehicles.json](../Garage/Data/vehicles.json) for complete examples of the vehicle types, and [the vehicle enums](../Vehicle/Vehicles/Types) for values such as colours and body types. Registration-format coverage is documented in [Validation.cs](../Vehicle/Vehicles/Validation.cs).

### Example: import an empty garage

Choose **Create garage from JSON files** and provide a layout file. Leave the parking assignments path blank to create an empty garage.

For unrestricted spots, the layout can be as small as:

```json
{
  "garageName": "Small Garage",
  "capacity": 5
}
```

For sized spots, save the following as `demo-garage.json`. Spots 1 and 2 use the default size; spot 3 uses the override:

```json
{
  "garageName": "Demo Garage",
  "capacity": 3,
  "defaultSpotSize": {
    "lengthMeters": 5,
    "widthMeters": 2.5,
    "heightMeters": 2.2
  },
  "spots": [
    {
      "spotNumber": 3,
      "size": {
        "lengthMeters": 12,
        "widthMeters": 5,
        "heightMeters": 5
      }
    }
  ]
}
```

Capacity must be between 1 and 100,000. Custom spot numbers must be unique and within capacity. Custom sizes require a `defaultSpotSize`, and every dimension must be positive and finite. Garage names must be unique in the running application.

### Example: load vehicles into specific spots

Keep the following files together with `demo-garage.json` from the previous example. Save this vehicle catalog as `demo-vehicles.json`. The square brackets are required because a placement file references a catalog, rather than a single vehicle object:

```json
[
  {
    "type": "Car",
    "registrationNumber": "DEM123",
    "countryCode": "SWE",
    "manufacturer": "Saab",
    "model": "9-3",
    "manufacturingYear": 2010,
    "color": "Blue",
    "weightKg": 1500,
    "lengthMeters": 4.6,
    "widthMeters": 1.8,
    "heightMeters": 1.5,
    "vin": "DEMO-SAAB-001",
    "bodyType": "Sedan",
    "numberOfDoors": 4,
    "numberOfSeats": 5,
    "engines": []
  }
]
```

Save this as `demo-placements.json`:

```json
{
  "garageName": "Demo Garage",
  "vehiclesFile": "demo-vehicles.json",
  "assignments": [
    { "spotNumber": 2, "registrationNumber": "DEM123" }
  ]
}
```

Choose **Create garage from JSON files**. Enter the path to `demo-garage.json`, then the path to `demo-placements.json`. The result is a three-spot garage with `DEM123` parked in spot 2.

`garageName` must match the layout name, ignoring letter case. `vehiclesFile` resolves relative to the placement file; it can also be an absolute path. Each assigned registration must exist in that catalog, each spot and registration may be assigned only once, and every assigned vehicle must fit. Only assigned vehicles become part of the imported garage. Registrations already in the running application are rejected.

The bundled [example-placements.json](../Garage/Data/example-placements.json) uses startup registrations, so importing those placements into the default console session will conflict. The `DEM123` example above uses a different registration and can be tried once in a fresh session.

## Common import problems

| Problem | Check |
| --- | --- |
| File cannot be found | Use an absolute path without surrounding quotes; check that the file exists. |
| Expected an object or an array | Single-vehicle import needs `{ ... }`; a placement's vehicle catalog needs `[ ... ]`. |
| Unknown property or enum value | Check spelling and use the names shown in the examples and enums. |
| Missing type-specific field | Add the field from the tables above, including zero counts and false booleans. |
| Duplicate registration or garage name | Use a value not already in this session. Comparisons ignore letter case. |
| Vehicle does not fit | Check all three vehicle dimensions against the assigned spot. |

## How the console is organized

[Program.cs](Program.cs) loads startup data and creates `GarageService`. [Menus](Menus) handles navigation and user actions, [Prompts](Prompts) reads and validates input, [Drafts](Drafts) holds unfinished creation input, [Mapping](Mapping) passes completed drafts to the factories, and [Display](Display) formats output. The shared `Garage` and `Vehicle` projects enforce the domain rules. The [Garage structure guide](../Garage/README.md) explains the services, queries, and results used by these menus.

All changes stay in memory for the current session. Imports do not overwrite their files, and quitting does not save garages or vehicles to JSON.
