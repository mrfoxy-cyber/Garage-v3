# Park vehicle

**Goal:** Put a vehicle in the garage when there is capacity available.

**Actor:** Garage user.

## Flow

1. The user requests parking from the console menu and supplies or selects a vehicle.
2. The system checks the vehicle details, registration uniqueness and available capacity.
3. The system adds the vehicle to the garage.
4. The system confirms that the vehicle is parked.

## Alternatives

- **Invalid details:** explain the problem and allow correction without crashing.
- **Duplicate registration:** reject the request and preserve the vehicles already parked.
- **Garage full:** explain that no capacity is available and leave the garage unchanged.

## Done when

- A valid vehicle is added when capacity is available.
- Parking does not exceed the chosen garage capacity or introduce a duplicate registration.
- A rejected request leaves the existing vehicles unchanged.
- The user receives a clear success or failure message.

Optional extensions are listed separately: [space selection, dimensions and filtering eligible vehicles](../optional/README.md). A separate catalogue and numbered parking-space objects are not required for basic parking.

**ID:** park-vehicle

**Scope:** Required

**Requirement references:** GAR-03, GAR-05, GAR-07, GAR-11, GAR-12, GAR-13
