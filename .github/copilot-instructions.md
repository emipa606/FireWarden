# GitHub Copilot Instructions for Fire Warden (Continued) Mod

## Mod Overview and Purpose

**Fire Warden (Continued)** is an expansion for the **Fire Extinguisher** mod, enhancing the gameplay by allowing pawns to use fire extinguishers as AI behavior. It introduces a new work column called "Fire Warden" (FW), providing flexible control over fire-fighting duties. The mod also includes various options for equipping and handling fire-fighting equipment, such as automatic equipping, swapping weapons, and setting the response range for fire situations.

## Key Features and Systems

### Work Column
- **Fire Warden (FW)**: A new work column that allows pawns to use fire extinguishers for fire-fighting duties.

### Equipment and Options
- **Fire Extinguisher (FE)**: Pawns can automatically equip and use fire extinguishers when a fire breaks out.
- **Firebeater (FB)**: A melee tool that can be researched and used by pawns for fire-fighting duties.
- **Automatic Equipping**: Pawns can be set to automatically equip fire-fighting equipment within a specified range.
- **Weapon Swapping**: Pawns will automatically swap to their fire-fighting equipment when a fire is detected and swap back after the threat is resolved.
- **Handling Options**: Players can set whether Fire Wardens respond to all fire situations or only when no other responder is nearby.

### AI Behavior
- **Bravery**: Pawns can be set to tackle fires more aggressively, regardless of other threats.
- **Foam Usage**: Fire extinguishers have a limited number of uses and will be destroyed when fully used up.

## Coding Patterns and Conventions

### C# Coding Standards
- **Class Naming**: Use PascalCase for class names.
- **Method Naming**: Use camelCase for method names.
- **Variable Naming**: Use camelCase for variable names.
- **Constants and Static Fields**: Use PascalCase for constants and static fields.
- **Comments**: Provide clear and concise comments for complex logic and new features.

### XML Integration
- **Def Files**: Use consistent naming and structure for XML definition files.
- **Mod Configurations**: Use XML to define mod options and settings.
- **Harmony Patches**: Use XML to define Harmony patches and their targets.

### Harmony Patching
- **Target Methods**: Clearly specify the target methods for Harmony patches.
- **Patch Logic**: Ensure that patch logic is efficient and does not introduce performance issues.
- **Testing**: Thoroughly test Harmony patches to ensure they work as expected.

## Suggestions for Copilot

### C# Snippets
- **Alerts**: Provide snippets for creating custom alerts, such as `Alert_FireWardenIsPyro` and `Alert_FireWardenLacksFE`.
- **Component Properties**: Provide snippets for defining custom component properties, such as `CompProperties_FWData`.
- **Harmony Patches**: Provide snippets for creating Harmony patches, including the necessary XML definitions.

### XML Snippets
- **Def Files**: Provide snippets for creating and configuring XML definition files, such as `Defs_Firebeater.xml` and `Defs_FireWarden.xml`.
- **Mod Configurations**: Provide snippets for defining mod options and settings in XML.
- **Harmony Patches**: Provide snippets for defining Harmony patches and their targets in XML.

### General Tips
- **Documentation**: Encourage Copilot to generate well-documented code with clear comments and explanations.
- **Testing**: Encourage Copilot to include testing code snippets to ensure that generated code works as expected.
- **Performance**: Encourage Copilot to consider performance implications when generating code, especially for Harmony patches.

## Example Usage

### C# Example: Creating a Custom Alert
csharp
public class Alert_FireWardenIsPyro : Alert
{
    public Alert_FireWardenIsPyro()
    {
        this.defaultLabel = "Fire Warden is Pyro";
        this.defaultExplanation = "A Fire Warden cannot perform fire-fighting duties due to their pyromania.";
    }

    public override TaggedString GetReport()
    {
        // Logic to determine if any Fire Warden is a pyro
        return "A Fire Warden is a pyro and cannot perform fire-fighting duties.";
    }
}


### XML Example: Defining a Firebeater
xml
<ThingDef>
    <defName>FWPrimDef_Firebeater</defName>
    <label>Firebeater</label>
    <description>A melee tool for fire-fighting duties.</description>
    <category>Weapon</category>
    <statBases>
        <MeleeDamage>5</MeleeDamage>
        <MeleeToHitOffset>0.2</MeleeToHitOffset>
    </statBases>
    <researchPrerequisites>
        <li>Mlie.Firebeater.Research</li>
    </researchPrerequisites>
</ThingDef>


### Harmony Patch Example: Patching Fire Extinguisher Usage
xml
<HarmonyPatch>
    <targetMethod>
        <declaringType>FireExtinguisher</declaringType>
        <methodName>Use</methodName>
    </targetMethod>
    <patchMethod>
        <declaringType>PelFireWarden.Patches.Patch_FireExtinguisher</declaringType>
        <methodName>Patch_Use</methodName>
    </patchMethod>
</HarmonyPatch>


## Conclusion

By following these coding patterns and conventions, and utilizing the provided snippets and examples, developers can efficiently create and maintain high-quality code for the Fire Warden (Continued) mod. Copilot can significantly assist in generating code that adheres to these standards, ensuring a seamless and effective modding experience.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

