# Use cases

These Markdown documents describe what a garage user wants to do and how the application should respond. Open them directly in an editor, Visual Studio, or the repository browser. Edit the documents by hand; no manager, generator, or diagram viewer is needed.

The [requirements checklist](requirements/README.md) records the existing Garage 1.0 assignment and the agreed use of lists instead of arrays. Required and optional describe scope, rather than implementation or test status.

## Required use cases

- [Create garage](garage/create-garage.md)
- [Park vehicle](garage/park-vehicle.md)
- [Remove vehicle](garage/remove-vehicle.md)
- [List parked vehicles](garage/list-vehicles.md)
- [List vehicle types and counts](garage/list-vehicle-types.md)
- [Find vehicle by registration](garage/find-vehicle.md)
- [Search parked vehicles](garage/search-vehicles.md)
- [Navigate garage menus](garage/navigate-menus.md)
- [Quit application](garage/quit-application.md)

## Optional use cases

- [Create garage with sized spaces](optional/create-garage-with-dimensions.md)
- [Create or import a catalogue vehicle](optional/create-vehicle.md)
- [Inspect parking spaces](optional/inspect-spaces.md)

See the [enhancement list](optional/README.md) for other optional ideas. [Implementation notes](implementation/README.md) link the current code and related tests.

## Read or add a use case

Each document has a goal, an actor, a numbered flow, alternatives, and observable completion conditions. Its ID, scope, and requirement references are ordinary documentation fields.

To add a use case, copy the [template](templates/use-case.md), write the complete scenario, and add a link to this index. Keep optional behavior separate from the required flow. Update documents and implementation links when behavior changes.

The documents are maintained manually. A documented use case or a link to a test does not by itself prove that the complete behavior is implemented or tested.
