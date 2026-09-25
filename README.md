# MazeEscape

A 2D horror maze game developed in Unity and C#.

## 🎮 About the Game

MazeEscape is a 2D horror game where the player must navigate through a randomly generated maze while avoiding a monster.

The monster can patrol the maze, detect the player, chase them, and search the player's last known position after losing sight of them.

## ✨ Features

- Procedurally generated maze
- Player movement
- Flashlight/2D lighting
- Monster AI
  - Patrol
  - Chase
  - Search
- Monster detection and line-of-sight system
- Monster pathfinding
- Player win condition
- Player caught/game-over condition
- Restart functionality
- Horror atmosphere and lighting

## 🔊 Audio

The game includes:

- Player footsteps
- Monster heavy footsteps
- Echoing footsteps
- Monster growl/detection sound
- Monster ambient breathing

## 👹 Monster AI

The monster has three main states:

### Patrol
The monster patrols the maze when the player has not been detected.

### Chase
When the monster detects the player, it follows them through the maze.

### Search
When the monster loses sight of the player, it searches around the player's last known position before returning to patrol.

## 🛠️ Technologies

- Unity
- C#
- Unity 2D
- Git
- GitHub
- Visual Studio Code
