# Inspect parking spaces

**Goal:** See each numbered parking space, its dimensions and whether it is occupied.

**Actor:** Garage user.

## Flow

1. The user selects a garage and requests its parking-space details.
2. The system retrieves the numbered spaces and occupancy information.
3. The system displays the available space details.

## Alternatives

- **No selected garage:** explain that a garage must be selected first.
- **Unsized space:** show the space and occupancy without inventing dimension limits.

## Done when

- Each space is listed once with its number and current occupancy.
- Configured dimensions are shown when available.
- Inspection leaves parking assignments unchanged.

**ID:** inspect-spaces

**Scope:** Optional

**Requirement references:** OPT-03, OPT-08
