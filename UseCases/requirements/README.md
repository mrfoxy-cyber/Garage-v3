# Garage 1.0 requirements

Source: the supplied **C# Övning 5 — Garage 1.0** [assignment text](source/garage-1.0-original.txt).

**Agreed change:** use `List<T>` for internal application collections, including the private garage collection. This replaces the original array requirement and its prohibition of `List<Vehicle>`. The original text is preserved unchanged for reference. A list's allocated capacity is not the garage's occupancy limit; the application must enforce the user-selected limit.

## Required scope

- **GAR-01 — Shared vehicle model:** use a `Vehicle` base class for properties shared by vehicle types. The assignment gives registration number, colour and wheel count as examples.
- **GAR-02 — Vehicle types:** provide at least `Motorcycle`, `Airplane`, `Car`, `Bus` and `Boat`, each with at least one property specific to that type.
- **GAR-03 — Unique registration:** a registration number uniquely identifies a vehicle.
- **GAR-04 — List storage:** keep collection storage private and list-based, following the agreed change above. The basic garage holds vehicles; an optional parking-space model may organize them through spaces instead.
- **GAR-05 — Garage capacity:** supply the garage's capacity when constructing it. Capacity is a positive number of vehicles; parking must respect that limit in the basic model.
- **GAR-06 — Initial vehicles:** support populating a garage with vehicles from the start. This capability does not require file import or persistence.
- **GAR-07 — Parking and removal:** add vehicles to the garage and take them out again.
- **GAR-08 — Listing:** show all parked vehicles and show the vehicle types with a count of each type.
- **GAR-09 — Registration lookup:** find a vehicle regardless of registration letter case, such as `ABC123`, `Abc123` or `AbC123`.
- **GAR-10 — Combined search:** search by one or any combination of shared `Vehicle` properties. Support the brief's type-filtered examples as well, such as motorcycles with a specified colour and wheel count. Searching subtype-specific properties is optional.
- **GAR-11 — Console navigation:** provide a text-based console UI with a main menu and submenus, access to all required garage functionality, garage creation with user-specified capacity, and a way to quit.
- **GAR-12 — Feedback:** confirm successful actions and explain rejected actions.
- **GAR-13 — Robust input:** handle invalid input and usage without crashing the application.

The positive-capacity and full-garage checks make the capacity requirement usable; the brief does not prescribe an upper limit such as 100,000. It also does not require a named garage, a separate vehicle catalogue, specific screen layouts or a particular UI library.

## Optional scope

The brief explicitly separates suggested extras from requirements. See [the enhancement list](../optional/README.md). Required stories should remain complete when those extras are absent.

## Traceability

Each required use-case document lists the relevant `GAR-*` IDs under **Requirement references**. These IDs identify requirements, while a document ID such as `create-garage` identifies a user goal. Links to related code and tests are maintained manually in the [implementation notes](../implementation/README.md); they do not change the scope of the requirements.
