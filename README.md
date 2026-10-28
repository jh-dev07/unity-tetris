# Pocket Blocks — offline Android falling-block game

A complete code-driven Unity project with a dark portrait interface, bright beveled pieces, large touch targets, a landing ghost, hold, three next-piece previews, seven-piece bags, wall kicks, lock delay, combos, increasing speed, local best scores, pause/resume, sound and optional vibration.

## Unity version

Use **Unity 6.3 LTS, 6000.3.26f1**. This is an established LTS release line, not the newest Unity feature release. The project pins this patch. Official release: https://unity.com/releases/editor/whats-new/6000.3.26f1 . A newer maintenance patch within 6000.3 is reasonable after testing.

## Open and play

1. Install Unity Hub, then Unity 6000.3.26f1 with **Android Build Support**, including **Android SDK & NDK Tools** and **OpenJDK**.
2. Extract this ZIP. In Hub choose **Add project from disk**, and select the `PocketBlocks` folder containing `Assets`, `Packages`, and `ProjectSettings`.
3. Let packages import and scripts compile. On first import, the editor setup creates `Assets/Scenes/Main.unity` and configures Android. Open that scene if it is not already open.
4. Press Play. Choose **NEW GAME**. All UI, geometry and audio are created locally at runtime. No manual prefab wiring, downloaded art, API keys, services or Asset Store imports are needed.
5. For a portrait preview set the Game view to 720 × 1280 or a phone aspect ratio.

Initial editor/module/package installation needs internet. **The installed game does not.** No ads, analytics, accounts, remote content, network libraries or monetization packages are included. The Android manifest explicitly removes INTERNET and ACCESS_NETWORK_STATE permissions.

## Build an APK

1. Choose **File → Build Profiles → Android → Switch Platform**.
2. Choose **Pocket Blocks → Configure Android**. This selects portrait orientation, Android 8.0/API 26 minimum, ARM64, IL2CPP, OpenGL ES 3 and the Unity Activity entry point. Input uses Unity's built-in Input Manager.
3. Choose **Pocket Blocks → Run Rules Checks** and check the Console for success.
4. Choose **Pocket Blocks → Build Android APK**.
5. The output is **`Builds/PocketBlocks.apk`**, inside this project. This is an APK, not an Android App Bundle. Transfer it to an ARM64 Android device, allow installation from your file manager if prompted, and install it.

The build command uses Unity's development/debug signing for convenient local installation. For distribution, configure your own keystore in Android Publishing Settings, and remove the `useCustomKeystore=false` line in `Assets/Editor/ProjectSetup.cs` so your signing choice is preserved. Keep the keystore and passwords outside source control. Change the package ID/company name if desired.

Batch build (replace the Unity executable and project paths):

```sh
/path/to/Unity -batchmode -quit -projectPath /absolute/path/PocketBlocks -buildTarget Android -executeMethod PocketBlocks.Editor.ProjectSetup.BuildApk -logFile /absolute/path/build.log
```

A licensed Unity installation with Android modules is required. The Android tools bundled with this Unity version are recommended; there is no separate Gradle installation to manage.

## Mobile controls

| Action | Control |
| --- | --- |
| Move | Left/right buttons; hold to repeat; or drag horizontally on the board |
| Rotate clockwise | TURN or tap the board |
| Soft drop | DOWN; hold to repeat; or drag downward |
| Instant drop | DROP or a long downward swipe (16% of screen height) |
| Store/swap piece | HOLD, once per falling piece |
| Pause | II or Android Back; automatic on losing focus/backgrounding |
| Continue later | CONTINUE on the title/pause screen |

Desktop testing: arrow keys move/soft drop, Up or X rotates, Space drops, C holds, Escape pauses. Keyboard soft drop is primarily a testing aid; mobile buttons have timed repeat.

## Rules and persistence

- Board: 10 × 20 visible cells plus two hidden spawn rows.
- Seven-piece shuffled bag; 3-piece visible queue; ghost landing indicator.
- Rotation uses forgiving horizontal and upward wall kicks. This is a casual rule set, not tournament SRS; there is no T-spin or back-to-back scoring.
- 0.5 second lock delay; movement/rotation can reset the delay up to 15 times per piece.
- Line scores: 100/300/500/800 × level; consecutive clearing pieces add 50 × combo × level. Soft drop gives 1 point/cell, hard drop 2 points/cell.
- Every ten lines increases the level; gravity accelerates to a capped interval.
- A blocked spawn or blocks remaining in the hidden rows ends the run.
- Current run saves after a lock/hold, pause and app exit, using local PlayerPrefs. Saves contain the board, active piece, held piece, next queue and remaining bag. Closing abruptly can lose movements since the last save. No cloud sync is involved; uninstalling/clearing app data removes progress.
- Sound and vibration preferences persist. Vibration defaults off.

## Code map

- `Assets/Scripts/BlocksEngine.cs`: Unity-independent rules and save validation.
- `Assets/Scripts/BoardGraphic.cs`: batched Unity UI geometry for board/pieces/previews.
- `Assets/Scripts/PocketGame.cs`: runtime interface, input, lifecycle, save and local synthesized sound.
- `Assets/Editor/ProjectSetup.cs`: first-open setup and APK build commands.
- `Assets/Editor/RulesChecks.cs`: runnable editor checks; no test-package dependency.
- `Assets/Plugins/Android/AndroidManifest.xml`: offline permission configuration.

## Validation status and device acceptance

The project was authored in an environment without Unity Editor or Android build tooling. No APK or Unity-rendered screenshot is included, and Android compilation, UI rendering and device performance have not been verified here. The pure C# rules compiled and passed the included checks plus 100 randomized games outside Unity. Run the rules checks in Unity as well before building. Review `VALIDATION.md` for the exact checks performed during authoring.

Before accepting a release:

- Import/compile, run the rules checks, then build the APK with no errors.
- Test on a 16:9 phone, a tall phone with a notch and a tablet; verify no controls overlap system UI and text stays readable.
- Install, enable airplane mode, relaunch and complete several games. Verify sound, vibration toggles, pause, background/resume, force-close/relaunch, best score and current-run continuation.
- Check movement repeat, rotation near both walls, HOLD restrictions, ghost/hard-drop agreement and game-over/restart.
- Inspect the merged Android manifest or run `apkanalyzer manifest permissions Builds/PocketBlocks.apk`; INTERNET and ACCESS_NETWORK_STATE must be absent.
- Check frame rate, battery use, touch accuracy and audio on physical hardware; the 60 FPS cap is a target rather than a measured result.

This project uses original source and a neutral app name. No third-party art/audio assets are bundled.
