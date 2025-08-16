# GitHub Copilot Instructions for Fire Warden (Continued) Modding Project

## Mod Overview and Purpose

**Mod Name: Fire Warden (Continued)**

The Fire Warden mod enhances the functionality of fire extinguishers in RimWorld by introducing a new AI behavior for pawns, allowing them to efficiently tackle fires while freeing up other colonists for different tasks. The mod provides a dedicated "Fire Warden" work column, offering flexible control over firefighting duties without affecting the existing fire fighting roles.

## Key Features and Systems

- **Dedicated "Fire Warden" Role**: Introduces a new work column for fire management, enabling pawns to prioritize firefighting tasks more effectively.
- **Automatic and Manual Equipping**: Allows for automatic or manual equipping of fire extinguishers (FEs) and Firebeaters (FBs), with customizable range options.
- **Weapon Swapping System**: Fire Wardens automatically swap to appropriate firefighting equipment during fires and revert to their original weapons once the threat is mitigated.
- **Flexible Fire Handling Options**: Customize pawn behavior to respond to specific fire situations and manage workload based on responder proximity.
- **Bravery Mode**: An experimental option that makes Fire Wardens prioritize aggressive firefighting, bypassing other threats.
- **Foam Usage Management**: Fire extinguishers have limited uses, requiring AI-driven replacement once depleted.
- **Firebeater**: An additional melee tool designed for effective fire handling, researchable during the neolithic period.

## Coding Patterns and Conventions

- **Class and Method Naming**: Follow PascalCase for class names and camelCase for method names. Private methods and members should be prefixed with an underscore.
- **Code Organization**: Separate functionalities into distinct classes, each encapsulating a specific aspect of the mod's logic.
- **Commenting**: Use XML documentation comments for public classes and methods for better readability and maintenance.

## XML Integration

- **Translation Files**: The mod supports multiple languages, including French, Chinese, and Russian. Ensure XML files for translations are updated alongside code changes.
- **Def Structure**: Utilize XML to define new work types, tools, and work giver parameters, integrating seamlessly with C# code for mod functionality.

## Harmony Patching

- **Purpose**: Utilize Harmony for safely patching existing methods in RimWorld without direct modification, ensuring compatibility with other mods.
- **Patch Classes**: Organize patches in a dedicated static class `HarmonyPatching.cs` to maintain separation between core functionality and mod integrations.

## Suggestions for Copilot

- **AI Behavior Development**: Leverage Copilot for creating complex AI behavior trees, especially for conditional fire responses and equipment handling.
- **Tool Equipping Logic**: Use Copilot to suggest code for automatically equipping and swapping FE and FB based on changing game conditions.
- **Translation Support**: Employ Copilot to streamline the process of expanding translation files with the correct localization keys.
- **Patch Suggestions**: Utilize Copilot to intelligently propose Harmony patches for new functionalities or updates to existing game methods.

By adhering to these guidelines and utilizing GitHub Copilot effectively, mod developers can enhance the Fire Warden mod's functionality and maintain code quality and readability across the project.


This instruction file provides a comprehensive guide to mod developers working on the "Fire Warden (Continued)" mod in RimWorld, detailing key systems, coding practices, and integration techniques essential for maintaining and expanding the mod effectively.
