# Search parked vehicles

**Goal:** Find parked vehicles matching one or several chosen properties.

**Actor:** Garage user.

## Flow

1. The user requests vehicle search from the console menu.
2. The user supplies one or more criteria using shared vehicle properties and, when wanted, vehicle type.
3. The system finds parked vehicles that match all supplied criteria.
4. The system displays the matching vehicles and their properties.

## Alternatives

- **One criterion:** search using just that criterion, such as colour.
- **Combined criteria:** apply the criteria together, such as black vehicles with four wheels, or pink motorcycles with three wheels.
- **No matches:** clearly report that no parked vehicles match.
- **Invalid values:** explain the problem and allow correction without crashing.

## Done when

- Each shared vehicle property can be used for searching, alone or in combination with the others.
- Type filtering can be combined with shared-property criteria.
- Every result matches all supplied criteria, and the garage remains unchanged.

Searching properties specific to a subclass is an [optional enhancement](../optional/README.md).

**ID:** search-vehicles

**Scope:** Required

**Requirement references:** GAR-01, GAR-10, GAR-11, GAR-12, GAR-13
