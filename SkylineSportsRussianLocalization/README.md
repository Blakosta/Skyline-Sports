# Skyline Sports — Russian localization add-on

This is a separate Cities: Skylines II mod assembly. It does not compile, replace, or patch the original Skyline Sports assembly.

## Current scope

- Registers a `ru-RU` localization dictionary.
- Resolves the original Skyline Sports settings object at runtime and uses its actual localization IDs.
- Translates the original mod's settings screen.
- Does not alter game simulation, schedules, teams, player generation, saves, or the original mod files.

## Known limitation

The original mod builds much of its in-game interface from literal strings in its C# UI data (for example, the Views layer). Those strings are not automatically translated by adding an `IDictionarySource`. This add-on therefore does **not** claim that the entire in-game sports interface is translated yet. A separate, verified runtime interception mechanism would be needed for those strings; it must be tested against the exact game and mod versions before being added.

Install alongside the original Skyline Sports mod. Do not replace its DLL.
