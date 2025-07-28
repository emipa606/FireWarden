# Copilot Instructions for RimWorld "Fire Warden" Mod Project

## Mod Overview and Purpose
"Fire Warden" is a mod designed to enhance the fire management capabilities of colonists in the game RimWorld. The mod introduces specialized roles and tools to better handle and prevent fires, thus improving the overall safety and efficiency of your colony. 

## Key Features and Systems
- **Fire Warden Alerts**: Specifically designed alerts for fire wardens, such as if a fire warden is a pyromaniac (`Alert_FireWardenIsPyro`) or lacks fire extinguishing tools (`Alert_FireWardenLacksFB` and `Alert_FireWardenLacksFE`).
- **Fire Warden Data and Components**: Custom components (`CompFWData`, `CompProperties_FWData`) to store and handle data related to fire warden tasks and equipment.
- **Job System Enhancements**: New job drivers (`JobDriver_FWEquipping`, `JobDriver_FEReplace`, etc.) tailored to fire warden roles, enabling efficient swapping and equipping of fire extinguishing tools.
- **Utility and Research**: Static utility classes (`FWFoamUtility`) and additional research branches (`FWResearch`) to enhance fire fighting capabilities.
- **Harmony Patching**: Modifies existing game behavior to integrate smoothly with core RimWorld mechanics via Harmony.

## Coding Patterns and Conventions
- **Namespaces and Class Organization**: Classes are organized around their function and purpose, with static utility methods housed in their respective classes, such as `FWFoamUtility`.
- **Consistent Naming**: Consistent prefix `FW` is used across classes and methods for fire warden related functionality to ensure clarity and organization.
- **Encapsulation of Functionality**: Private helper methods within job drivers to encapsulate and isolate specific behaviors (e.g., `private bool IsFW(Pawn p)`).

## XML Integration
- **Defining Custom Work Types and Tools**: Use XML to define new work types using `FWWorkTypeDef` to integrate with RimWorld’s job system.
- **Customization of Gizmos**: XML is used to define and customize the appearance of new gizmos for fire warden tools, as seen in `Gizmo_FEFoamStatus`.

## Harmony Patching
- **Patch Integration**: The class `HarmonyPatching` is responsible for applying runtime modifications to existing RimWorld factions and character behavior, ensuring seamless integration.
- **Target-Method Patching**: Use of [Harmony](https://harmony.pardeike.net/) to dynamically alter methods like `Pawn_DraftController_GetGizmos` to add custom fire warden actions.

## Suggestions for Copilot
1. **Autocomplete and Suggestions**:
   - Suggest potential improvements or refactorings for job driver logic when new cases arise.
   - Offer XML templates for defining or modifying existing game entities related to fire management.

2. **Patch Scaffolding**:
   - Assist in generating basic patching code via Harmony. Suggest patches for integrating new tools and tasks with existing game mechanics.
   
3. **Tool Utilities**:
   - Generate utility methods for common tasks in fire management, potentially optimizing the performance or simplifying code logic.

4. **Code Analysis**:
   - Provide insights or suggest corrections when writing complex logic for fire warden job allocation or pathfinding algorithms.

By leveraging GitHub Copilot, RimWorld mod developers can streamline their workflow, ensure consistency across the codebase, and integrate seamlessly with the core game mechanics. These instructions provide a foundation for AI-assisted development in this mod project.

---

For any new development, refer to these instructions to maintain consistency and efficiency in extending the "Fire Warden" mod.
