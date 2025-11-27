# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## IMPORTANT: Code Assistance Policy

**DO NOT directly modify files in this project.** When the user asks for code changes:
- Provide the code changes as text in your response
- Explain what changes need to be made and where
- Let the user copy and paste the code themselves
- You may read files to understand the codebase, but do not use Edit or Write tools

**Exception:** When the user explicitly and directly requests you to modify a file (e.g., "너가 수정해줘", "add this to the file"), you should:
- Use Edit or Write tools to make the changes
- Verify the changes are appropriate and well-formed
- Suggest improvements if the requested change seems problematic

## Project Overview

ArcadeLab is a multiplayer 2D Unity game featuring a central lobby where players can customize characters and join various arcade game rooms. The client is built with Unity 2022.3+ using C# and communicates with a Node.js backend via Socket.IO for real-time multiplayer functionality.

## Development Commands

### Unity Editor
- Open the project in Unity Editor (version 2022.3 or higher)
- The project uses Hot Reload for Unity (Packages/com.singularitygroup.hotreload) for faster iteration

### Backend Server
The client expects a Socket.IO server running at `http://localhost:3000`. The server must support two namespaces:
- Root namespace (`/`) for authentication
- Lobby namespace (`/lobby`) for game lobby operations

## Architecture Overview

### Dependency Injection with VContainer

The project uses **VContainer** for dependency injection with a scene-based lifetime scope pattern:

**Lifetime Scope Hierarchy:**
```
ApplicationLifetimeScope (Startup scene - persists across scenes)
├── AuthManager (DontDestroyOnLoad)
└── AuthNetworkService

MainMenuLifetimeScope (MainMenu scene)
└── MainMenuUI

LobbyLifetimeScope (Lobby scene)
├── LobbyManager
├── LobbyUIManager
├── RoomManager
└── LobbyNetworkService

ColorLabLifetimeScope / other game scopes
└── Game-specific services
```

**Key DI Patterns:**
- Use `[Inject]` attribute for constructor/property injection
- Network services are registered as Singletons
- Managers are registered as Components (scene-scoped)
- All services implement `VContainer.Unity.IInitializable` for lifecycle management

### Networking Architecture

The project uses **Socket.IO Unity Client** with Newtonsoft JSON serialization.

**Network Services Pattern:**
All network services implement `INetworkService` interface:
```csharp
public interface INetworkService
{
    void Initialize();
    void RegisterEventListeners();
    Task ConnectAsync();
    void Disconnect();
}
```

**Socket.IO Connections:**
1. **AuthNetworkService** - Connects to `http://localhost:3000` for authentication
2. **LobbyNetworkService** - Connects to `http://localhost:3000/lobby` with userId in auth header

**Event Naming Convention:**
- Format: `[entity]:[action]` (e.g., `player:move`, `room:create`)
- All listeners use `OnUnityThread()` to ensure Unity main thread execution
- Events are exposed as C# events for loose coupling

### Scene Flow

```
Startup (ApplicationLifetimeScope initializes)
    ↓
MainMenu (Guest authentication)
    ↓
Lobby (Player spawning, character customization, room browsing)
    ↓
Game Scene (ColorLab, DessertHunter, etc.)
```

### Code Organization

```
Assets/Scripts/
├── Character/          # Player character logic and skin management
├── Data/              # DTOs and data classes (LobbyData, CommonData, etc.)
├── GamePortal/        # Base classes for game integration
├── LifetimeScope/     # VContainer DI configuration per scene
├── Managers/          # High-level game logic (AuthManager, LobbyManager, RoomManager)
├── Network/           # Socket.IO network services
└── UI Components      # UI panels and controllers
```

## Key Architectural Patterns

### Manager Pattern
Managers coordinate between network layer and game logic:
- **AuthManager**: Handles authentication flow and scene transitions (Application-scoped, persists)
- **LobbyManager**: Synchronizes player state (spawning, movement, customization)
- **RoomManager**: Manages game room creation and listing
- **LobbyUIManager**: Coordinates UI panel visibility

### Event-Driven Communication
- Network services expose C# events (e.g., `OnPlayerConnected`, `OnRoomCreated`)
- Managers subscribe to these events in their initialization
- PlayerController uses events for movement/skin/nickname changes
- Maintains loose coupling between layers

### Player Synchronization
Each PlayerController has an `IsOwner` flag:
- **Owner (local player)**: Processes input, emits network events
- **Remote players**: Updates position/state from network events

Movement synchronization flow:
```
Local Input → PlayerController.OnMoved event → LobbyManager → Network Emit
→ Server Broadcast → All Clients → LobbyManager.OnPlayerMoved
→ PlayerController.UpdateRemotePosition() (non-owner only)
```

## Data Layer Conventions

All data classes are in `Assets/Scripts/Data/`:
- **CommonData.cs**: Shared types (`Position`, `PlayerBase`)
- **LobbyData.cs**: Lobby-specific DTOs (`LobbyPlayerData`, `RoomData`, etc.)
- **[GameName]Data.cs**: Game-specific data classes

Data classes are serializable POCOs matching server-side schemas exactly.

## Network Protocol

### Player Events (Lobby)

**Client Emits:**
- `player:move` → `Position` (throttled at 0.01s)
- `player:skin` → `int skinIndex`
- `player:nickname` → `string nickname`

**Client Listens:**
- `player:connect` → `LobbyPlayerData` (new player joins)
- `player:others` → `LobbyPlayerData[]` (initial player list)
- `player:move` → `PlayerMoveData` (player position update)
- `player:join` → `LobbyPlayerData` (player joins after initial connection)
- `player:leave` → `string userId` (player disconnects)
- `player:skin` → `PlayerSkinData` (skin change)
- `player:nickname` → `PlayerNicknameData` (nickname change)

### Room Events

**Client Emits:**
- `room:create` → `CreateRoomRequest(gameId, roomName, maxPlayers)`

**Client Listens:**
- `room:create` → `RoomData` (room created)
- `room:delete` → `string roomId` (room deleted)

## Adding New Features

### Adding a New Game
1. Create game scene in `Assets/Scenes/[GameName].unity`
2. Create `[GameName]LifetimeScope.cs` in `Assets/Scripts/LifetimeScope/`
3. Create `[GameName]Data.cs` in `Assets/Scripts/Data/` with game-specific DTOs
4. Create `GameConfig` ScriptableObject asset for the game
5. Implement game-specific managers and register them in the LifetimeScope

### Adding a New Network Event
1. Add event handler to appropriate NetworkService (`LobbyNetworkService`, etc.)
2. Add C# event declaration at top of service class
3. Register listener in `RegisterEventListeners()` using `OnUnityThread()`
4. Subscribe to event in appropriate Manager class
5. Add corresponding data class to Data layer if needed

### Adding a New Manager
1. Create manager class in `Assets/Scripts/Managers/`
2. Inject required dependencies via constructor with `[Inject]` attribute
3. Register as Component in appropriate LifetimeScope
4. Subscribe to relevant network events in Start/initialization

## Communication Guidelines

When providing technical assistance:
- **Provide ONE recommended solution with full code implementation**
- **Briefly mention alternative approaches** without detailed code
- Focus on the most practical and maintainable solution
- Explain trade-offs only when the recommended approach has significant limitations

Example response structure:
1. Recommended approach (with complete code)
2. Brief mentions of alternatives (1-2 sentences each)
3. Why the recommended approach is preferred

## Important Notes

- All Socket.IO event listeners MUST use `OnUnityThread()` to avoid threading issues
- Use `DontDestroyOnLoad` only for Application-scoped services (AuthManager, AuthNetworkService)
- Network serialization uses Newtonsoft.Json - ensure data classes are JSON-serializable
- PlayerController.IsOwner determines whether to process input or sync from network
- Server URL is hardcoded to `http://localhost:3000` in network services
- The project uses Unity's new Input System (InputSystem_Actions.inputactions)
- Player sprite libraries support 4 skins (index 0-3) defined in PlayerLibraryData ScriptableObject

## Known Issues / TODOs

- MatchMakingNetworkService is a placeholder (not yet implemented)
- RoomDetail class exists but is not used
- ColorLab and DessertHunter game logic not yet implemented
- No in-game room logic (ready states, team assignment, match start)
- GameConfig has typo: `gmaeId` instead of `gameId`
