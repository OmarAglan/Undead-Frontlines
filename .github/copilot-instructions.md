# Copilot Instructions for Undead Frontlines

## Project Overview
Undead Frontlines is a Unity multiplayer zombie survival game built with **FishNet 4.6.12** networking. The project emphasizes multiplayer gameplay with dedicated server architecture, featuring authentication, scene management, and real-time networking.

## Essential Architecture Knowledge

### FishNet Networking Stack
- **Core Pattern**: Inherit from `NetworkBehaviour` for networked objects, use `TickNetworkBehaviour` for prediction-based systems
- **NetworkManager**: Central hub managing ServerManager, ClientManager, TransportManager, TimeManager, and SceneManager
- **Prediction**: Use `[Replicate]` methods for client input and `[Reconcile]` for server-authoritative corrections
- **RPC Pattern**: `[ServerRpc]` for client→server calls, `[ObserversRpc]` for server→clients broadcasts

### Project Structure
```
Assets/
├── _Scripts/           # Custom game code with clear namespace organization
│   ├── API/           # Backend integration (ApiManager with JWT auth)
│   ├── Network/       # FishNet integration (NetworkUIController)
│   └── Main Menu/     # UI controllers and authentication
├── _Scenes/           # Game scenes: Main Menu, GamePlay, SafeZone
├── _Prefabs/          # Currently empty - prefabs likely stored elsewhere
└── FishNet/           # Complete FishNet framework (v4.6.12)
```

## Development Patterns

### Networking Code Standards
1. **Namespace Convention**: Use `UndeadFrontlines.Network` for custom networking code
2. **NetworkBehaviour Template**:
   ```csharp
   using UnityEngine;
   using FishNet.Object;
   
   namespace UndeadFrontlines.[Module]
   {
       public class [ClassName] : NetworkBehaviour
       {
           public override void OnStartClient() { }
           public override void OnStartServer() { }
       }
   }
   ```

3. **Event Subscription Pattern**: Always pair `OnEnable`/`OnDisable` for FishNet events:
   ```csharp
   private void OnEnable()
   {
       networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
   }
   
   private void OnDisable()
   {
       networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
   }
   ```

### API Integration
- **Authentication**: Uses `ApiManager` singleton with JWT tokens for backend communication
- **Async Pattern**: API calls use `async Task<T>` with `await` for Unity coroutine integration
- **Error Handling**: Check `response.success` boolean before proceeding with API responses

### UI Architecture
- **TextMeshPro**: Standard for all UI text elements (`TMP_InputField`, `TextMeshProUGUI`)
- **Component Validation**: Always validate UI references with null checks and meaningful error messages
- **Scene Flow**: Main Menu → Authentication → GamePlay scenes with FishNet scene management

## Critical Development Workflows

### FishNet Integration
1. **NetworkManager Setup**: Must be present in each scene, configured with Transport (Tugboat default port 7770)
2. **Server/Client Logic**: Use `IsServerStarted`/`IsClientStarted` for role-specific behavior
3. **Connection State Handling**: Subscribe to connection state changes for UI updates and game state management

### Testing & Debugging
- **Multiple Instances**: Test with separate server/client instances
- **Host Mode**: Server + Client on same instance for development
- **Transport Layer**: Default Tugboat transport handles low-level networking

### Scene Management
- Use FishNet's `SceneManager` for multiplayer scene transitions
- Scenes: Main Menu (authentication) → GamePlay (main game) → SafeZone (possible lobby/safe area)

## Key Dependencies & Integrations
- **FishNet**: Complete networking solution with prediction, object pooling, and scene management
- **Newtonsoft.Json**: JSON serialization for API communication  
- **TextMeshPro**: UI text rendering
- **Unity Input System**: Not yet implemented (using legacy Input.GetAxisRaw)

## Common Pitfalls & Guidelines
1. **NetworkBehaviour Lifecycle**: Initialize network components in `OnStartServer`/`OnStartClient`, not `Awake`/`Start`
2. **Authority Checks**: Always verify `IsOwner`, `IsServerStarted`, or `IsClientStarted` before network operations
3. **Scene Persistence**: Use `DontDestroyOnLoad` for NetworkManager and persistent managers
4. **Input Handling**: Current implementation uses legacy input system - consider Unity's new Input System for production

## Specific to This Project
- **Game Genre**: Zombie survival with multiplayer focus
- **Authentication Flow**: Email/password → JWT → Game scenes
- **Network UI**: Dedicated controller for server/client management in development
- **Backend Integration**: Localhost API (port 4000) for user management

Focus on FishNet-specific networking patterns, proper client-server authority, and maintaining the existing authentication and scene management architecture when extending this codebase.