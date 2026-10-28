# Validation performed

- Compiled the actual `BlocksEngine.cs` rules source with Mono C# compiler 6.8: passed.
- Executed every check in the actual `Assets/Editor/RulesChecks.cs`, with only the Unity logging and MenuItem attribute replaced by standalone stubs: passed. These cover seven-piece bag uniqueness, hold restrictions, ghost landing, left-wall collision, wall rotation, four-row clearing/scoring, save restoration, corrupt-save rejection and top-out.
- Executed 100 seeded random games with 7,972 total actions: all passed active-piece collision and snapshot restoration invariants.
- Parsed all seven C# source files using the Tree-sitter C# grammar: no syntax errors.
- Parsed the package manifest JSON and Android manifest XML; verified explicit network permission removal and the required project files.
- Checked the final ZIP integrity.

The standalone test runner is `Tests/StandaloneRunner.cs` (outside Assets, so its Unity stubs cannot conflict with Unity). With Mono installed, run from the project root:

```sh
mcs -out:/tmp/PocketRules.exe Assets/Scripts/BlocksEngine.cs Assets/Editor/RulesChecks.cs Tests/StandaloneRunner.cs
mono /tmp/PocketRules.exe
```

## Still requires Unity/device testing

Unity was not installed in the authoring environment. Unity API binding, package import, Android Gradle/IL2CPP compilation, rendered UI, actual sound/vibration, application lifecycle, permission merge, frame rate and physical-device controls were not executed. No APK is included. Use the README acceptance checklist before distribution.
