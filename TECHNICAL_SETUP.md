# TECHNICAL SETUP GUIDE

## Backend Setup

### Prerequisites
- Node.js 18+
- Docker Desktop
- Git

### Installation Steps
```bash
# Clone repository
cd backend

# Install dependencies
npm install

# Start PostgreSQL in Docker
docker-compose up -d

# Run database migrations
npx prisma migrate dev

# Import world data
npm run import-world

# Start server
npm run dev
```

### Environment Variables (.env)
```env
DATABASE_URL="postgresql://postgres:postgres@localhost:5432/undead_frontlines"
JWT_SECRET="your-secret-key-change-in-production"
JWT_REFRESH_SECRET="your-refresh-secret-key"
PORT=3000
```

## Unity Setup

### Prerequisites
- Unity 2022.3 LTS
- Visual Studio 2022 / VS Code

### Project Configuration
1. Open Unity Hub
2. Add existing project
3. Open with Unity 2022.3

### Required Packages
- Fish-Networking (Asset Store)
- TextMeshPro
- Universal RP
- Cinemachine (optional)

### Build Settings
- Platform: PC, Mac & Linux Standalone
- Architecture: x86_64
- Development Build: Yes (for testing)

## Docker Configuration

### docker-compose.yml
```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: undead_frontlines
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## Network Ports
- Backend API: 3000
- PostgreSQL: 5432
- Game Server: 7770
- Unity Editor: 7771-7779 (testing)