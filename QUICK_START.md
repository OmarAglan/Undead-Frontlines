# QUICK START GUIDE

## Start Everything Up

### 1. Start Backend
```bash
cd backend
docker-compose up -d  # Start PostgreSQL
npm run dev          # Start server on :3000
```

### 2. Open Unity Project
- Open Unity Hub
- Select project: undead-frontlines/unity-client
- Open SafeZone scene

### 3. Test Connection
- Play in Editor
- Click "Start Server"
- Click "Start Client"
- Verify connection status

## Continue Development

### Current Task: Player Movement
1. Open `PlayerMovement.cs`
2. Attach to PlayerPrefab
3. Configure NetworkObject
4. Test movement

### Next Task: Combat System
1. Create WeaponBase.cs
2. Implement shooting
3. Add hit detection
4. Test multiplayer combat

## Common Commands

### Backend
```bash
npm run dev           # Start development server
npm run import-world  # Import world data
npx prisma studio    # Open database GUI
npx prisma migrate dev # Run migrations
```

### Unity
```
Ctrl+P - Play/Stop
Ctrl+S - Save Scene
Ctrl+Shift+B - Build Settings
F5 - Refresh Assets
```

## Troubleshooting

### Backend Not Connecting
- Check Docker is running
- Verify PostgreSQL on :5432
- Check .env file exists
- Run migrations: `npx prisma migrate dev`

### Unity Network Issues
- NetworkManager exists in scene
- Tugboat transport configured
- Port 7770 not blocked
- Firewall allows Unity

### Database Issues
- Reset: `npx prisma migrate reset`
- Import: `npm run import-world`
- View: `npx prisma studio`

## Key Files Locations

Backend:
- API Routes: `backend/src/routes/`
- Database: `backend/prisma/schema.prisma`
- Config: `backend/.env`

Unity:
- Scripts: `Assets/_Scripts/`
- Prefabs: `Assets/_Prefabs/`
- Scenes: `Assets/_Scenes/`

## Support Resources

- FishNet Docs: https://fish-networking.gitbook.io/
- Unity Multiplayer: https://docs.unity3d.com/
- Prisma Docs: https://www.prisma.io/docs/