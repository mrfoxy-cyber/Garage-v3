# List parked vehicles

**Goal:** See which vehicles are currently parked and their properties.

**Actor:** Garage user.

## Flow

1. The user requests the list of parked vehicles from the console menu.
2. The system retrieves the vehicles currently in the garage.
3. The system displays each vehicle's registration, type and vehicle properties.

## Alternatives

- **Empty garage:** explain that no vehicles are parked.

## Done when

- Every parked vehicle is included once.
- The user can distinguish vehicles and inspect their properties.
- Vehicles outside the garage are not included.
- Listing leaves the garage unchanged.

**ID:** list-vehicles

**Scope:** Required

**Requirement references:** GAR-01, GAR-02, GAR-08, GAR-11, GAR-12
