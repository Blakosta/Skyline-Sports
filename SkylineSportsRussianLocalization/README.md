# Skyline Sports — Russian localization add-on

This is a separate Cities: Skylines II mod assembly. It does not rebuild, replace, or modify the original Skyline Sports DLL.

## What it translates

- The original Skyline Sports settings screen.
- Text values sent by the sports UI through the original mod's JSON builder, using a runtime Harmony prefix.
- A growing Russian dictionary for sports tabs, records, player bios, international competitions, news and dates.

The patch is attached only to `Skyline_Sports_Project.Engine.JsonBuilder.Value(string)`. It does not patch simulation, scheduling, player/team generation, saves, or game logic.

## Install

1. Keep the original Skyline Sports mod installed.
2. Install this add-on as a separate mod.
3. Copy both `SkylineSportsRussianLocalization.dll` and `0Harmony.dll` from the build artifact into the add-on's mod folder.
4. Enable both Skyline Sports and Skyline Sports Russian Localization, then select Russian in the game language settings.

Never replace the original `Skyline_Sports_Project.dll`.

## Compatibility and current scope

The runtime translation hook is version-sensitive: it expects the original mod to expose `Skyline_Sports_Project.Engine.JsonBuilder.Value(string)`. If the original mod changes that API, the patch may not attach. Check the game log for:

`[Skyline Sports RU] Runtime UI text translation attached.`

This is the first pass of the interface dictionary, not a claim that every dynamically composed sentence or every name is translated. Missing strings can be added to the dictionary without changing the original mod.


## Runtime translation coverage

The standalone patch translates exact UI values emitted by Skyline Sports' `JsonBuilder`, including navigation labels, tournament descriptions, standings terminology, awards, and selected dynamic announcements. It also localizes country labels and group headings where they are emitted as standalone values. It also handles selected transfer, roster, and promotion/relegation news templates while preserving player and club names. It deliberately avoids blanket substitutions inside player names, club names, and arbitrary generated text. Simulation rules, player-generation probabilities, and game mechanics remain untouched.


Newly generated players are kept on the Russian name pools already present in the original project. Generated high-school team names receive a Russian school suffix. These are runtime content-localization patches; no original source files or simulation rules are modified.
