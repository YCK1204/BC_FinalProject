# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2D action platformer game project (BC_FinalProject) built with Unity 6000.2.2f1. The project features a player character that can battle various monsters, collect items, and unlock traits in a 2D side-scrolling environment.

## Development Commands

### Unity Development
- **Open Project**: Open the project folder in Unity Hub or directly launch Unity with this directory
- **Build Game**: Use Unity's Build Settings (File > Build Settings) or use Addressables build system
- **Run Tests**: Use Unity Test Runner (Window > General > Test Runner)

### C# Development
- **Open in IDE**: Open `BC_FinalProject.sln` or `Final.sln` in Visual Studio/Rider
- **Compile Scripts**: Unity automatically compiles scripts on save or manually via Assets > Reimport All

## Core Architecture

### Manager System
The project uses a singleton manager pattern located in `Assets/_Scripts/Managers/`:
- **Manager.cs**: Central singleton that manages all other managers
- **GameManager**: Handles game state, monster counting, and level progression
- **ResourceManager**: Handles resource loading and asset management
- **AudioManager**: Manages audio systems
- **PoolManager**: Object pooling for performance optimization
- **DataManager**: Handles data persistence and ScriptableObject management
- **ItemManager**: Manages item collection, inventory, and item effects
- **PlayerManager**: Manages player-specific functionality

### Player System
Located in `Assets/_Scripts/Player/`:
- **State Machine Pattern**: PlayerStateMachine with states for Idle, Walk, Jump, Attack, Dash, etc.
- **PlayerCharacter.cs**: Main player controller with HP, awakening system, and combat
- **Trait System**: Complex trait/skill tree system with passive effects and item synergies

### Monster System
Located in `Assets/_Scripts/Monster/`:
- **Base Classes**: BaseMonster provides core functionality for all enemies
- **State Machines**: MonsterStateMachine with Attack, Chase, Idle, Patrol, Hit, Die states
- **Behavior Trees**: Advanced AI for boss monsters (BossMonster, BoneReaper)
- **Attack System**: Modular attack patterns (Melee, Ranged, Homing attacks)
- **Spawning**: Area-based and automated monster spawning systems

### Item System
Located in `Assets/_Scripts/UI/Item/` and data in `Assets/_Scripts/ScriptableObject/`:
- **Google Sheets Integration**: Uses `com.shlifedev.ugs` package for data management
- **ScriptableObject Data**: ItemData, ItemBuffData, ItemProjectileData, ItemAreaData
- **Synergy System**: Items can combine for enhanced effects
- **Data Readers**: Automated data loading from Google Sheets

### UI Architecture
Located in `Assets/_Scripts/UI/`:
- **State Pattern**: UI screens use state machines (BackState, FullScreenState, etc.)
- **HUD System**: Health bars, awakening gauge
- **Inventory System**: Item display and management
- **Trait UI**: Complex skill tree interface

## Key Features

### Combat System
- Player has normal and awakened states with different animations
- Multiple attack types: basic attacks, air attacks, dash attacks
- Hit detection with knockback and damage calculation
- Critical hit system and invincibility frames

### Awakening System
- Gauge fills on taking damage
- Temporary power boost with enhanced attack range
- Visual effects and animation controller switching

### Data Management
- ScriptableObjects for all game data (monsters, items, projectiles)
- Google Sheets integration for external data management
- Addressables system for asset loading

### Monster AI
- State machines for basic monsters
- Behavior trees for complex boss encounters
- Modular attack and movement systems
- Detection and chase behaviors

## Important Directories

- `Assets/_Scripts/`: All C# scripts organized by functionality
- `Assets/_Prefabs/`: Game object prefabs organized by type
- `Assets/Scenes/`: Unity scene files for different levels
- `Assets/Arts/`: Sprites, animations, and visual assets
- `Assets/Data/ScriptableObject/`: Game data assets

## Package Dependencies

Key packages used:
- **Addressables**: Asset management and loading
- **Cinemachine**: Camera system
- **Input System**: Modern input handling
- **URP**: Universal Render Pipeline for 2D graphics
- **DOTween**: Animation tweening
- **Timeline**: Cutscenes and scripted sequences
- **Google Sheets Integration**: External data management

## Build Configuration

- Primary build target: PC/Mac/Linux
- Uses Addressables for content delivery
- URP configured for 2D rendering
- Input System configured with action maps

## Development Notes

- The project uses namespace organization (e.g., `Game.Player`, `Game.Monster`)
- Extensive use of Unity's new Input System
- State machine patterns for both player and monster AI
- Manager classes follow singleton pattern with proper initialization order
- ScriptableObject-based data architecture for easy content modification