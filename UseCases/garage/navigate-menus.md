# Navigate garage menus

**Goal:** Reach the required garage functions through the console interface.

**Actor:** Garage user.

## Flow

1. The system presents the main menu and its available choices.
2. The user selects a function or submenu.
3. The system opens the requested interaction and shows the available choices.
4. The user can continue with a function or return to the main menu.

## Alternatives

- **Invalid choice:** explain the available choices and allow another selection without crashing.
- **A garage is needed:** explain that a garage must first be created before the requested operation can continue.

## Done when

- All required functions are reachable through text-based main and submenus.
- Garage creation with user-specified capacity and application exit are reachable.
- Invalid navigation does not crash the application or change garage contents.

Screen layouts, keyboard shortcuts and a rich dashboard are separate implementation choices or [optional enhancements](../optional/README.md).

**ID:** navigate-menus

**Scope:** Required

**Requirement references:** GAR-11, GAR-12, GAR-13
