# UNITY IMPLEMENTATION GUIDE

## Player Controller Setup

### 1. Create Player Prefab
```
PlayerPrefab (GameObject)
├── Model (Capsule/Character Mesh)
│   └── CharacterController component
├── CameraRig (Empty)
│   └── CameraFollowPoint (Empty)
│       └── PlayerCamera (Camera)
├── GroundCheck (Empty at feet)
├── WeaponHolder (Empty)
│   └── WeaponMount (Empty)
└── UI
    └── NameCanvas (World Space Canvas)
        └── NameText (TextMeshPro)
```

### 2. Required Components
- NetworkObject (FishNet)
- NetworkTransform (FishNet)
- CharacterController (Unity)
- PlayerMovement (Custom)
- PlayerCombat (Custom)
- PlayerInventory (Custom)

### 3. Layer Setup
```
Layers:
- Default (0)
- TransparentFX (1)
- Ignore Raycast (2)
- Water (4)
- UI (5)
- Player (6) [Custom]
- Enemy (7) [Custom]
- Ground (8) [Custom]
- Weapon (9) [Custom]
- Interactable (10) [Custom]
```

### 4. Input System Setup
```csharp
Input Manager:
- Horizontal: A/D, Left/Right arrows
- Vertical: W/S, Up/Down arrows
- Jump: Space
- Sprint: Left Shift
- Crouch: Left Control
- Fire1: Left Mouse Button
- Fire2: Right Mouse Button (Aim)
- Reload: R
- Interact: E
- Inventory: Tab
- Menu: Escape
```

## Network Configuration

### NetworkManager Settings
```
NetworkManager Component:
- Persistence: Don't Destroy On Load
- Run In Background: ✓
- Refresh Default Prefabs: ✓
- Default Spawnable Prefabs:
  - PlayerPrefab
  - ZombiePrefab
  - ItemDropPrefab

Tugboat Transport:
- Port: 7770
- Client Address: localhost
- MTU: 1200
- Timeout: 15
```

### Scene Network Setup
```
Each Scene Needs:
1. NetworkManager (persistent)
2. SpawnPoints (Empty GameObjects)
3. NetworkUI (for debugging)
```

## Animation Setup

### Animator Controller States
```
Base Layer:
├── Idle
├── Walk (blend tree)
├── Run
├── Jump
├── Crouch Idle
├── Crouch Walk
└── Death

Upper Body Override:
├── Aim
├── Shoot
├── Reload
└── Melee
```

### Animation Parameters
```
Float: MoveSpeed (0-1)
Float: Direction (-1 to 1)
Bool: IsGrounded
Bool: IsCrouching
Bool: IsAiming
Trigger: Jump
Trigger: Shoot
Trigger: Death
```

## Combat System

### Weapon Types
```csharp
public enum WeaponType
{
    Pistol,      // Starting weapon
    Rifle,       // Automatic
    Shotgun,     // Close range
    Sniper,      // Long range
    Melee        // Knife/Bat
}
```

### Damage Calculation
```csharp
Damage = BaseDamage * DamageMultiplier * (1 - TargetArmor/100)
Headshot Multiplier = 2.0
Critical Hit Chance = 5% base
```

## UI System Structure

### HUD Elements
```
Game HUD:
├── Health Bar
├── Stamina Bar
├── Ammo Counter
├── Minimap
├── Objective Tracker
├── Kill Feed
├── Chat Box
└── Interaction Prompt
```

### Menu Screens
```
MainMenu
├── Login Panel
├── Register Panel
├── Server Browser
└── Settings

CharacterSelection
├── Character List
├── Create New Button
├── Character Preview
└── Enter World Button

Inventory
├── Character Model
├── Equipment Slots
├── Inventory Grid
├── Item Details
└── Stats Display
```

## Performance Guidelines

### Optimization Targets
- 60 FPS minimum on mid-range hardware
- Network updates: 30Hz for movement, 10Hz for stats
- LOD distances: 0-25m (High), 25-50m (Medium), 50m+ (Low)
- Max zombies per instance: 50
- Max players per Safe Zone: 32
- Max players per mission: 4

### Best Practices
1. Use object pooling for projectiles/effects
2. Batch draw calls where possible
3. Compress textures appropriately
4. Use LODs for all models
5. Occlusion culling in all scenes
6. Minimize Update() calls - use events
7. Server authoritative for all important logic