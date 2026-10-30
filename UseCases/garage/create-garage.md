# Create garage

**Goal:** Make a garage available with the capacity chosen by the user.

**Actor:** Garage user.

## Flow

1. The user requests a new garage from the console menu.
2. The user enters its capacity.
3. The system validates the capacity and creates an empty garage with that limit.
4. The system confirms that the garage is ready to use.

## Alternatives

- **Invalid capacity:** explain the problem and allow correction without crashing or creating an invalid garage.
- **Initial vehicles:** the application must also support creating a garage populated from the start. Initial vehicles must satisfy the same validity, uniqueness and capacity rules as vehicles parked later. The assignment does not specify how those initial vehicles are supplied.

## Done when

- A valid request creates a garage with the chosen capacity.
- Its vehicle collection is stored privately using a list.
- It can start empty or with valid initial vehicles without exceeding capacity.
- Invalid input is handled with clear feedback.

Optional extensions are listed separately: [multiple garages, configuration, file loading and sized spaces](../optional/README.md). Names, dimensions and JSON files are not required inputs to this basic flow.

**ID:** create-garage

**Scope:** Required

**Requirement references:** GAR-03, GAR-04, GAR-05, GAR-06, GAR-11, GAR-12, GAR-13
