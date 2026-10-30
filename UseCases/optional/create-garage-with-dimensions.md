# Create garage with sized spaces

**Goal:** Create a garage whose spaces have individual length, width and height limits.

**Actor:** Garage user.

## Flow

1. The user supplies the garage name and the dimensions of its parking spaces.
2. The system validates the name and space dimensions.
3. The system registers the garage with the supplied spaces and confirms success.

## Alternatives

- **Invalid dimensions:** explain the problem and leave the garage collection unchanged.
- **Duplicate name:** reject creation and preserve the existing garage.

## Done when

- Every created space retains its configured dimensions.
- A rejected request does not add a partial garage.
- Sized spaces remain an optional extension of the basic capacity-based garage.

**ID:** create-garage-with-dimensions

**Scope:** Optional

**Requirement references:** OPT-08
