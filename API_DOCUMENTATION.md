# API DOCUMENTATION

## Base URL
```
http://localhost:3000/api
```

## Authentication

### Register User
```http
POST /auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "username": "player123",
  "password": "SecurePass123!"
}

Response: 201
{
  "message": "User created successfully",
  "userId": "uuid"
}
```

### Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}

Response: 200
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "user": {
    "id": "uuid",
    "username": "player123",
    "email": "user@example.com"
  }
}
```

### Refresh Token
```http
POST /auth/refresh
Content-Type: application/json

{
  "refreshToken": "refresh-token"
}

Response: 200
{
  "accessToken": "new-jwt-token"
}
```

## World Data

### Get Countries
```http
GET /world/countries
Authorization: Bearer {token}

Response: 200
[
  {
    "id": 1,
    "name": "United States",
    "code": "US",
    "status": "CONTESTED",
    "liberationProgress": 45.5
  }
]
```

### Get Governorates
```http
GET /world/countries/{countryId}/governorates
Authorization: Bearer {token}

Response: 200
[
  {
    "id": 1,
    "name": "California",
    "countryId": 1,
    "status": "INFECTED",
    "liberationProgress": 12.3
  }
]
```

### Get Cities
```http
GET /world/governorates/{governorateId}/cities
Authorization: Bearer {token}

Response: 200
[
  {
    "id": 1,
    "name": "Los Angeles",
    "governorateId": 1,
    "population": 3900000,
    "status": "INFECTED",
    "progress": 0,
    "latitude": 34.0522,
    "longitude": -118.2437
  }
]
```

## Player Management

### Get Profile
```http
GET /player/profile
Authorization: Bearer {token}

Response: 200
{
  "id": "uuid",
  "userId": "uuid",
  "level": 10,
  "experience": 2500,
  "currency": 1000,
  "characters": [
    {
      "id": "uuid",
      "name": "JohnWick",
      "class": "SOLDIER",
      "level": 10
    }
  ],
  "safeZoneHost": "game-server-1.example.com"
}
```

### Create Character
```http
POST /player/character
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "JohnWick",
  "class": "SOLDIER",
  "customization": {
    "gender": "male",
    "face": 1,
    "hair": 3,
    "hairColor": "#000000"
  }
}

Response: 201
{
  "id": "uuid",
  "name": "JohnWick",
  "class": "SOLDIER",
  "level": 1,
  "experience": 0
}
```

## Mission System

### Join City Mission
```http
POST /world/cities/{cityId}/join
Authorization: Bearer {token}

Response: 200
{
  "missionServer": "mission-3.example.com",
  "port": 7770,
  "token": "mission-jwt-token"
}
```

### Complete Mission
```http
POST /player/missions/complete
Authorization: Bearer {token}
Content-Type: application/json

{
  "missionId": "uuid",
  "results": {
    "success": true,
    "zombiesKilled": 45,
    "objectivesCompleted": 3,
    "experienceEarned": 500,
    "loot": ["item1", "item2"]
  }
}

Response: 200
{
  "rewards": {
    "experience": 500,
    "currency": 100,
    "items": ["item1", "item2"]
  },
  "newLevel": 11,
  "newExperience": 3000
}
```