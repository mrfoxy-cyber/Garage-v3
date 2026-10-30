# Remove vehicle

**Goal:** Take a parked vehicle out of the garage and make its capacity available again.

**Actor:** Garage user.

## Flow

1. The user requests vehicle removal from the console menu.
2. The user identifies the parked vehicle.
3. The system finds the vehicle and removes it from the garage.
4. The system confirms that the vehicle has been removed.

## Alternatives

- **Vehicle not found:** explain that no matching vehicle is parked and leave the garage unchanged.
- **Invalid input:** explain the problem and allow correction without crashing.

## Done when

- The identified vehicle is no longer parked in the garage.
- Its capacity is available for another vehicle.
- Other parked vehicles remain unchanged.
- Registration lookup ignores letter case, and success or failure is clearly reported.

**ID:** remove-vehicle

**Scope:** Required

**Requirement references:** GAR-07, GAR-09, GAR-11, GAR-12, GAR-13
