# Tower_Defence


A Unity/C# tower defence prototype with procedural enemy path generation, free turret placement, enemy waves, projectile systems, and protect tower health logic.

This project was originally developed as a tower defence game during my Bachelor's studies and later extended with new gameplay logic, including procedural path generation so that a new enemy path is created each time the game restarts.

## Project Overview

The game is built around a protect-tower objective. Enemies spawn from an enemy tower and follow a generated path toward the protect tower. The player places turrets freely on the map to stop the enemies before they reach the target.

Unlike a fixed-grid tower defence game, this prototype uses procedural path generation and raycast-based turret placement, making each play session slightly different.

## Features

- Procedural enemy path generation on each game restart
- Enemy spawning and path-following movement
- Protect tower health system and game-over logic
- Free turret placement using mouse raycasting
- Placement validation to prevent turrets from being placed on enemy paths
- Minimum-distance checks between placed turrets
- UI-based turret selection through a shop system
- Standard turret and missile launcher turret types
- Bullet and missile projectile behaviour
- Explosion and particle effects on impact
- Turret repositioning after placement
- 3D health bar for the protect tower
- Modular C# scripts for gameplay systems


## Gameplay

The player selects a turret type from the UI and places it on a valid position on the map. Turrets automatically detect and shoot enemies within range. Enemies follow the generated path toward the protect tower. If enemies reach the tower, the tower loses health.

## Controls

| Action | Control |
|---|---|
| Select turret | UI button |
| Place turret | Left mouse click |
| Move placed turret | Click turret, then click a valid position |

## Core Systems

### Procedural Path Generation

A new enemy path is generated each time the game starts or restarts. This increases replayability and requires the turret placement system to adapt to the current path layout.

### Turret Placement

The project uses raycasting from the camera to the placement plane. The system checks whether the selected position is valid before allowing turret placement.

Validation includes:

- Blocking placement on or near the enemy path
- Preventing turrets from being placed too close to each other
- Updating the placement preview based on validity

### Turret Combat

Turrets automatically search for enemies within range and rotate toward the selected target. Different turret types use different projectile prefabs, including bullets and missiles.

### Protect Tower Health

The protect tower has a health system with a 3D health bar. When enemies reach the end of the path, the tower takes damage. The game ends when the tower health reaches zero.


## Setup

1. Clone the repository

2. Open the project in Unity.

3. Open the main scene from the `Assets/Scenes` folder.

4. Press Play to run the prototype.


## Future Improvements

- Add turret upgrades and economy system
- Upgrade the scene or environment
- Add wave progression and difficulty scaling
- Improve path visuals with a continuous road system
- Add WebGL or itch.io playable build
