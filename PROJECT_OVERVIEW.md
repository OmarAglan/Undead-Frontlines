# UNDEAD FRONTLINES - PROJECT OVERVIEW

## Vision
A multiplayer post-apocalyptic survival game where players fight to reclaim real-world territories from zombies.

## Core Features
- Persistent world based on real geography (250 countries, 150,000+ cities)
- Hybrid architecture: Backend (Node.js) + Game Servers (Unity/FishNet)
- Territory liberation system with global progression
- Safe zones and instanced missions
- Character progression and customization

## Technical Architecture

### Backend Server (Brain)
- **Purpose:** Handles persistent data, authentication, world state
- **Tech:** Node.js, Express, PostgreSQL, Prisma, JWT
- **Port:** 3000
- **Status:** ✅ COMPLETE

### Game Servers (Reflexes)  
- **Purpose:** Real-time gameplay, physics, combat
- **Tech:** Unity 2022.3 LTS, Fish-Networking
- **Port:** 7770
- **Status:** 🔄 IN DEVELOPMENT

### Client
- **Platform:** PC (expandable to mobile)
- **View:** Third-person (First-person planned)
- **Status:** 🔄 IN DEVELOPMENT

## Player Flow
1. Launch game → Main Menu
2. Login/Register → Backend authentication
3. Character Selection/Creation
4. Tutorial (new characters)
5. Safe Zone (social hub)
6. Mission Selection → Instance gameplay
7. Return to Safe Zone → Progress saved

## Project Philosophy
- Incremental development - playable at each milestone
- Network-first design - everything synchronized
- Real geography for immersion
- Community-driven territory liberation