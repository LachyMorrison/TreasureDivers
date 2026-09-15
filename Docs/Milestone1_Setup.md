# TreasureDivers Milestone 1 Setup

This milestone adds a first-person single-player diver foundation using Unity's current Input System.

## One-click scene setup

1. Open Unity.
2. Open `Assets/_Project/Scenes/Prototype_DiveRoom.unity`.
3. In the top menu, click `Tools > Treasure Divers > Milestone 1 > Configure Prototype Dive Room`.
4. Save the scene.
5. Press Play.

The setup command configures:

- `Player` with a `CharacterController`, `DiverInputReader`, `FirstPersonDiverMotor`, `WaterSensor`, `DiverCursorLock`, and `PlayerInteractor`.
- `Main Camera` as the player's first-person camera.
- `TestTreasure` with `BasicTreasureInteractable`.
- `WaterVolume_Prototype` as a trigger volume around the underwater area.

## Validation in Unity

After running setup, click `Tools > Treasure Divers > Milestone 1 > Validate Prototype Dive Room Setup`.

Unity should log that Milestone 1 validation passed.

## Controls

- `Mouse`: look around
- `WASD`: move
- `Space`: jump while grounded
- `Space`: swim upward while in water
- `Left Ctrl`: swim downward while in water
- `E`: interact with an object in front of the camera
- `Escape`: release the cursor
- `Left mouse click`: lock the cursor again

## Tuning

Select `Player`, then adjust values on `FirstPersonDiverMotor`:

- Mouse sensitivity
- Walk speed
- Jump height
- Gravity
- Ground check radius and distance
- Swim speed
- Swim vertical speed
- Swim transition speed

## Notes

- This does not add multiplayer networking yet.
- Input is isolated in `DiverInputReader` so a later multiplayer milestone can replace local input with per-player authority.
- Interaction only establishes the raycast and `IInteractable` contract. Treasure pickup and carrying are intentionally left for a later milestone.
