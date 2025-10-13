# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2D action platformer game project (BC_FinalProject) built with Unity 6000.2.2f1. The project is a bootcamp final project ("내배캠 11기 최종 프로젝트") featuring a player character that can battle various monsters, collect items, and unlock traits in a 2D side-scrolling environment.

## Development Commands

### Unity Development
- **Open Project**: Open the project folder in Unity Hub or directly launch Unity with this directory
- **Build Game**: Use Unity's Build Settings (File > Build Settings) or use Addressables build system (Window > Asset Management > Addressables)
- **Run Tests**: Use Unity Test Runner (Window > General > Test Runner) - limited test coverage currently exists
- **Asset Management**: Use Addressables Groups window to manage asset bundles and loading

### C# Development
- **Open in IDE**: Open `BC_FinalProject.sln` or `Final.sln` in Visual Studio/Rider
- **Compile Scripts**: Unity automatically compiles scripts on save or manually via Assets > Reimport All
- **Manager Initialization**: Central Manager.cs handles initialization order: Data → Pool → Item → Physics2D layer setup

## Core Architecture

### Manager System
The project uses a singleton manager pattern with centralized initialization in `Assets/_Scripts/Managers/Manager.cs`:
- **Manager.cs**: Central singleton that manages all other managers with DontDestroyOnLoad lifecycle
- **Initialization Order**: Critical sequence - Data.Load() → Pool.Init() → Item.Init() → Physics2D layer collision setup
- **GameManager**: Handles game state, monster counting, and level progression
- **ResourceManager**: Handles resource loading and asset management  
- **AudioManager & SceneManagerEx**: Scene and audio management (set via properties with transform parenting)
- **PoolManager**: Object pooling for performance optimization
- **DataManager**: Handles data persistence and ScriptableObject management
- **ItemManager**: Manages item collection, inventory, and item effects
- **Static Access Pattern**: All managers accessed via `Manager.ManagerName` (e.g., `Manager.Game`, `Manager.Data`)

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
- **Google Sheets Integration**: Uses `com.shlifedev.ugs` package (external Git dependency) for data management
- **Complex Data Architecture**: ItemData, ItemBuffData, ItemProjectileData, ItemAreaData, ItemSynergyData, ItemDescriptionData, ItemEffectData
- **Synergy System**: Items can combine for enhanced effects with dedicated synergy data structure
- **Data Readers**: Automated parsing from Google Sheets to Unity data structures with runtime dictionary access
- **Buff Management**: Separate BuffManager for item effect application and management

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

Critical packages and versions:
- **com.shlifedev.ugs**: Google Sheets integration (external Git dependency from GitHub)
- **com.unity.addressables (2.7.2)**: Asset management and loading system
- **com.unity.render-pipelines.universal (17.2.0)**: Universal Render Pipeline for 2D graphics
- **com.unity.inputsystem (1.14.2)**: Modern input handling with action maps
- **com.unity.cinemachine (3.1.4)**: Advanced camera system
- **com.unity.timeline (1.8.9)**: Cutscenes and scripted sequences
- **2D Animation Package Suite**: Complete 2D animation pipeline (animation, aseprite, psd importer, sprite tools)
- **com.unity.test-framework (1.5.1)**: Testing infrastructure (limited current usage)

## Build Configuration

- Primary build target: PC/Mac/Linux
- Uses Addressables for content delivery
- URP configured for 2D rendering
- Input System configured with action maps

## Development Notes

### Code Organization & Patterns
- **Namespace Organization**: `Game.Player`, `Game.Monster` for logical separation and avoiding naming conflicts
- **Interface-Driven Design**: Consistent use of interfaces like `IDamageable`, `IAttackable`, `IMovable`, `IStateMachine`
- **State Machine Architecture**: Robust implementation for both player and monster AI systems
- **Manager Singleton Pattern**: Centralized management with proper initialization sequence and lifecycle
- **ScriptableObject Data Architecture**: Extensive use for game data with external Google Sheets integration

### Input System Configuration
- Modern Unity Input System with comprehensive action maps
- Configured Player actions: Move, Look, Attack, Interact, Crouch, Jump, Dash
- Supports multiple input methods and devices

### Team Collaboration
- **Korean Development Team**: Comments and documentation in Korean, code in English
- **GitHub Workflow**: Structured PR templates with Korean descriptions and categorized change types
- **Minimal Current Documentation**: Only basic README with project name "내배캠 11기 최종 프로젝트"

### Testing & Quality
- **Limited Test Coverage**: Only one test file (`EnemyTest.cs`) for basic monster damage testing
- **Editor Development Tools**: Custom Gizmos for monster detection and attack ranges
- **Debug Systems**: Test triggers and development utilities available

### Performance Considerations
- **Object Pooling**: Implemented via PoolManager for frequently instantiated objects
- **Addressable Asset System**: For efficient asset loading and memory management
- **Physics2D Optimization**: Specific layer collision matrix setup (Player ignores Destructible layer)

## Language Settings
- **Development Language Note**: 나는 한국인이니까 앞으로 한국어로 답해 (As I am Korean, I will respond in Korean from now on)