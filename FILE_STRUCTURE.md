# PROJECT FILE STRUCTURE

## Root Directory
```
undead-frontlines/
├── backend/
├── unity-client/
├── docs/
├── shared/
└── README.md
```

## Backend Structure
```
backend/
├── prisma/
│   ├── schema.prisma
│   └── migrations/
├── src/
│   ├── routes/
│   │   ├── auth.js
│   │   ├── world.js
│   │   └── player.js
│   ├── middleware/
│   │   └── auth.js
│   ├── services/
│   │   ├── authService.js
│   │   └── worldService.js
│   ├── utils/
│   │   └── jwt.js
│   └── app.js
├── scripts/
│   └── import-world-data.js
├── data/
│   ├── countries.json
│   ├── states.json
│   └── cities.json
├── .env
├── .gitignore
├── docker-compose.yml
└── package.json
```

## Unity Structure
```
unity-client/
├── Assets/
│   ├── _Scenes/
│   │   ├── MainMenu.unity
│   │   ├── CharacterCreation.unity
│   │   ├── Tutorial.unity
│   │   ├── SafeZone.unity
│   │   └── Missions/
│   │       └── ClearOutpost.unity
│   ├── _Scripts/
│   │   ├── Network/
│   │   │   ├── NetworkManager.cs
│   │   │   ├── NetworkUIController.cs
│   │   │   └── NetworkDebugConsole.cs
│   │   ├── Player/
│   │   │   ├── PlayerMovement.cs
│   │   │   ├── ThirdPersonCamera.cs
│   │   │   ├── PlayerCombat.cs
│   │   │   └── PlayerInventory.cs
│   │   ├── Combat/
│   │   │   ├── WeaponBase.cs
│   │   │   ├── ProjectileWeapon.cs
│   │   │   └── DamageHandler.cs
│   │   ├── UI/
│   │   │   ├── MainMenuUI.cs
│   │   │   ├── CharacterCreationUI.cs
│   │   │   └── HUDManager.cs
│   │   ├── API/
│   │   │   ├── ApiService.cs
│   │   │   ├── AuthManager.cs
│   │   │   └── Models/
│   │   └── AI/
│   │       ├── ZombieAI.cs
│   │       └── AISpawner.cs
│   ├── _Prefabs/
│   │   ├── Player/
│   │   │   └── PlayerPrefab.prefab
│   │   ├── Weapons/
│   │   ├── Enemies/
│   │   ├── UI/
│   │   └── Environment/
│   ├── _Materials/
│   ├── _Textures/
│   ├── _Models/
│   ├── _Animations/
│   └── FishNet/
├── ProjectSettings/
├── Packages/
└── .gitignore
```

## Shared Resources
```
shared/
├── GameConstants.cs
├── NetworkMessages.cs
└── DataModels.cs
```