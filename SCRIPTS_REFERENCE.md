# KEY SCRIPTS REFERENCE

## PlayerMovement.cs
```csharp
// Handles all player movement and input
// Location: Assets/_Scripts/Player/PlayerMovement.cs

Key Methods:
- GatherInput() - Collects input from player
- SendMovementInput() - ServerRpc to send input
- ServerMove() - Server-side movement processing
- HandleCameraRotation() - Camera orbit control
- InterpolatePosition() - Smooth non-owner positions

Properties:
- walkSpeed: 5f
- sprintSpeed: 8f
- jumpForce: 8f
- mouseSensitivity: 2f
```

## ThirdPersonCamera.cs
```csharp
// Camera follow and collision system
// Location: Assets/_Scripts/Player/ThirdPersonCamera.cs

Key Methods:
- LateUpdate() - Camera positioning
- CheckCollision() - Prevent wall clipping
- HandleZoom() - Scroll wheel zoom

Properties:
- distance: 5f
- minDistance: 2f
- maxDistance: 10f
- collisionRadius: 0.3f
```

## NetworkUIController.cs
```csharp
// UI for network connections
// Location: Assets/_Scripts/Network/NetworkUIController.cs

Key Methods:
- StartServer() - Initialize server
- StartClient() - Connect to server
- StopConnection() - Disconnect
- UpdateButtonStates() - UI state management

Events:
- OnServerConnectionState
- OnClientConnectionState
```

## ApiService.cs (To Implement)
```csharp
// Backend communication
// Location: Assets/_Scripts/API/ApiService.cs

Key Methods:
- Login(email, password)
- Register(email, username, password)
- GetPlayerProfile()
- CreateCharacter(characterData)
- GetWorldData()
- JoinMission(cityId)

Properties:
- baseUrl: "http://localhost:3000/api"
- authToken: JWT storage
```

## WeaponBase.cs (To Implement)
```csharp
// Base weapon functionality
// Location: Assets/_Scripts/Combat/WeaponBase.cs

Abstract Methods:
- Fire()
- Reload()
- Aim()

Properties:
- damage: float
- fireRate: float
- ammoCapacity: int
- currentAmmo: int
- reloadTime: float
```

## ZombieAI.cs (To Implement)
```csharp
// Enemy AI behavior
// Location: Assets/_Scripts/AI/ZombieAI.cs

States:
- Idle
- Patrol
- Chase
- Attack
- Dead

Key Methods:
- DetectPlayer()
- PathfindToTarget()
- AttackPlayer()
- TakeDamage()
```