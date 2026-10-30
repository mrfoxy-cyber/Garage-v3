# Optional enhancements

These capabilities may extend the required garage. Their presence in the application does not make them assignment requirements. They are candidate extensions; this list does not claim they are implemented or fully tested.

## Suggested by the assignment

- **OPT-01 — Search type-specific properties:** extend search beyond shared `Vehicle` properties.
- **OPT-02 — Multiple garages:** manage and switch between garages, potentially for different vehicle types. Clearly show the active garage and apply actions to it.
- **OPT-03 — Parking-space model:** represent spaces that can hold vehicles, instead of only a garage collection of vehicles.
- **OPT-04 — Save and load:** save garage state to files and restore it through the menu or at shutdown/startup.
- **OPT-05 — Different space consumption:** let vehicles consume different amounts of capacity, such as two spaces for a boat or one third for a motorcycle.
- **OPT-06 — Eligible choices:** when parking, offer only vehicles the garage has room for.
- **OPT-07 — Configured capacity:** read the garage size from configuration.

## Additional project ideas

- **OPT-08 — Sized spaces:** give spaces length, width and height, with defaults and individual overrides. This is distinct from the assignment's fractional or multiple-space consumption idea.
- **OPT-09 — Choose a space:** let the user request a particular numbered space and reject unavailable choices.
- **OPT-10 — JSON import:** import layouts and optional placements. This can implement part of file loading; import alone does not establish full save-and-load support.
- **OPT-11 — Rich terminal dashboard:** add forms, tables, keyboard shortcuts and a richer layout beyond the required console menus.
- **OPT-12 — Shared vehicle catalogue:** manage parked and unparked vehicles separately and support creating a vehicle before parking it.

When an enhancement is selected for development, give it a short optional story or an optional scenario linked to its parent goal. It must not silently add conditions to a required story. For example, sized spaces can extend [Create garage](../garage/create-garage.md), while specific-space selection can extend [Park vehicle](../garage/park-vehicle.md).

Mark its scope as **Optional** in the document. Record implementation and tests in [implementation notes](../implementation/README.md).
