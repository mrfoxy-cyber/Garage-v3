# List vehicle types and counts

**Goal:** See how many vehicles of each type are parked in the garage.

**Actor:** Garage user.

## Flow

1. The user requests a summary by vehicle type from the console menu.
2. The system groups the parked vehicles by type and counts each group.
3. The system displays the vehicle types present and their counts.

## Alternatives

- **Empty garage:** explain that no vehicles are parked.

## Done when

- Each displayed count reflects the vehicles currently parked of that type.
- The counts add up to the number of parked vehicles.
- Vehicles outside the garage are not counted.
- Viewing the summary leaves the garage unchanged.

**ID:** list-vehicle-types

**Scope:** Required

**Requirement references:** GAR-02, GAR-08, GAR-11, GAR-12
