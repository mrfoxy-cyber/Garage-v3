# Quit application

**Goal:** End the running garage application through its interface.

**Actor:** Garage user.

## Flow

1. The user chooses the exit option from the console menu.
2. The system ends the menu interaction and shuts down normally.

## Alternatives

- **Invalid menu choice:** keep the application running, explain the available choices and allow another selection.

## Done when

- The user can end the application without forcibly closing the console.
- Choosing exit terminates the running application normally.

Saving garage state between runs is an [optional enhancement](../optional/README.md); this use case does not require persistence or a confirmation prompt.

**ID:** quit-application

**Scope:** Required

**Requirement references:** GAR-11, GAR-13
