# Create or import catalogue vehicle

**Goal:** Add a valid vehicle to the shared catalogue before parking it, with optional immediate parking.

**Actor:** Garage user.

## Flow

1. The user enters vehicle details or selects a vehicle JSON file.
2. The system validates the vehicle and checks registration uniqueness.
3. If the user selected a garage, the system attempts to park the vehicle there.
4. The system adds the vehicle to the catalogue and confirms success.

## Alternatives

- **Invalid details or file:** explain the problem and leave the catalogue unchanged.
- **Duplicate registration:** reject the vehicle and preserve the existing catalogue.
- **Parking rejected:** explain the failure and do not register the new vehicle in the catalogue.

## Done when

- Successful creation adds exactly one valid vehicle with a unique registration.
- A vehicle may remain unparked in the catalogue.
- Immediate parking must succeed before catalogue registration is committed.

**ID:** create-vehicle

**Scope:** Optional

**Requirement references:** OPT-10, OPT-12
