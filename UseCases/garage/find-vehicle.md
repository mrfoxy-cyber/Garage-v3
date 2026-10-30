# Find vehicle by registration

**Goal:** Find a specific parked vehicle using its registration number.

**Actor:** Garage user.

## Flow

1. The user requests registration lookup from the console menu.
2. The user enters a registration number.
3. The system searches the parked vehicles without distinguishing uppercase and lowercase letters.
4. The system displays the matching vehicle and its properties.

## Alternatives

- **No match:** explain that no vehicle with that registration is parked.
- **Invalid input:** explain the problem and allow correction without crashing.

## Done when

- `ABC123`, `Abc123` and `AbC123` find the same vehicle.
- The result identifies the matching parked vehicle, or clearly reports no match.
- Searching leaves the garage unchanged.

**ID:** find-vehicle

**Scope:** Required

**Requirement references:** GAR-03, GAR-09, GAR-11, GAR-12, GAR-13
