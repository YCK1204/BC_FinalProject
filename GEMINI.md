# GEMINI.md

## Project Overview

This is a 2D action game developed in Unity. The project is named "BC_FinalProject" and appears to be a final project for a "Naebaecamp" course.

**Core Technologies:**

*   **Game Engine:** Unity (version 6000.2.2f1)
*   **Programming Language:** C#

**Architecture:**

*   The project follows a singleton pattern for manager classes like `GameManager` and `MapManager`.
*   The player character is implemented using a state machine pattern, which is a common and effective way to manage complex character behaviors.
*   The code is organized into namespaces (e.g., `Game.Player`, `Game.Monster`) to maintain a clean and organized structure.

## Building and Running

To build and run this project, you will need to have Unity version **6000.2.2f1** installed.

1.  Open the project in the Unity Hub.
2.  Once the project is open in the Unity Editor, you can run the game by pressing the "Play" button.

**Key Scenes:**

*   `Assets/Scenes/Play.unity`: This appears to be the main game scene.
*   `Assets/Scenes/Map_cwy_BossAdd_KNJ.unity`: This might be a boss level or a more advanced version of the main game scene.
*   Other scenes in `Assets/Scenes` appear to be for testing specific features.

## Development Conventions

*   **Coding Style:** The code is written in C# and generally follows standard C# and Unity conventions.
*   **State Machine:** The player character uses a state machine to handle different states like idle, walking, jumping, attacking, and taking damage. This is a good practice for managing complex character logic.
*   **Singleton Managers:** The game uses singletons for manager classes. This can be a convenient way to access global systems, but it's important to be mindful of the potential drawbacks of this pattern.
*   **Events:** The code uses C# events (`Action`) for communication between different game components. This helps to decouple the code and make it more modular.
*   **Data Serialization:** The project uses `[SerializeField]` to expose variables to the Unity Inspector, allowing for easy tweaking of game parameters.
