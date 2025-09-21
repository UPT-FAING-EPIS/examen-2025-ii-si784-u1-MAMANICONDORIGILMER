# Music Platform API

Una API REST simple para gestión de suscripciones a plataforma de música desarrollada en .NET Core.

## Características

- ✅ Catálogo de música (canciones con artista y duración)
- ✅ Gestión de suscripciones (Free y Premium)
- ✅ Sistema de playlists personalizadas
- ✅ Reportes de uso y suscripciones
- ✅ Conexión a SQL Server

## Configuración

### Base de Datos
La API está configurada para conectarse a la base de datos `MusicPlatformSimpleDB` en SQL Server local.

1. Ejecuta el script SQL proporcionado para crear la base de datos y las tablas
2. Modifica la cadena de conexión en `appsettings.json` si es necesario:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MusicPlatformSimpleDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Ejecutar la aplicación
```bash
dotnet run
```

La API estará disponible en `https://localhost:7000` (HTTPS) y `http://localhost:5000` (HTTP).

## Endpoints de la API

### 🎵 Music Controller (`/api/music`)

#### Listar todas las canciones
- **GET** `/api/music`
- Respuesta: Lista de canciones con formato de duración MM:SS

#### Obtener canción específica
- **GET** `/api/music/{id}`
- Respuesta: Detalles de la canción incluyendo duración en segundos

#### Buscar canciones
- **GET** `/api/music/search?artist={artist}&title={title}`
- Parámetros opcionales: `artist`, `title`

### 📋 Subscriptions Controller (`/api/subscriptions`)

#### Ver suscripción de usuario
- **GET** `/api/subscriptions/{userId}`
- Respuesta: Suscripción activa del usuario

#### Crear o renovar suscripción
- **POST** `/api/subscriptions`
- Body:
```json
{
  "userId": 1,
  "planType": "Premium"
}
```

#### Cancelar suscripción
- **DELETE** `/api/subscriptions/{id}`

#### Ver planes disponibles
- **GET** `/api/subscriptions/plans`
- Respuesta: Lista de planes Free y Premium con características

### 🎶 Playlists Controller (`/api/playlists`)

#### Listar playlists de usuario
- **GET** `/api/playlists/{userId}`
- Respuesta: Playlists del usuario con canciones incluidas

#### Crear playlist
- **POST** `/api/playlists`
- Body:
```json
{
  "userId": 1,
  "name": "Mi Playlist"
}
```

#### Agregar canción a playlist
- **POST** `/api/playlists/{playlistId}/songs`
- Body:
```json
{
  "songId": 5
}
```

#### Remover canción de playlist
- **DELETE** `/api/playlists/{playlistId}/songs/{songId}`

#### Ver detalles de playlist
- **GET** `/api/playlists/{id}/details`
- Respuesta: Información completa de la playlist incluyendo duración total

### 📊 Reports Controller (`/api/reports`)

#### Reporte de suscripciones
- **POST** `/api/reports/subscriptions`
- Respuesta: Estadísticas de suscripciones por plan y actividad reciente

#### Reporte de uso
- **POST** `/api/reports/usage`
- Respuesta: Artistas populares, canciones más agregadas a playlists

#### Reporte de usuarios
- **POST** `/api/reports/users`
- Respuesta: Actividad de usuarios y estadísticas generales

## Estructura del Proyecto

```
MusicPlatformAPI/
├── Controllers/
│   ├── MusicController.cs
│   ├── SubscriptionsController.cs
│   ├── PlaylistsController.cs
│   └── ReportsController.cs
├── Data/
│   └── MusicDbContext.cs
├── Models/
│   ├── User.cs
│   ├── Song.cs
│   ├── Subscription.cs
│   ├── Playlist.cs
│   └── PlaylistSong.cs
├── Program.cs
└── appsettings.json
```

## Modelos de Datos

### User
- Id, Email (único), Name

### Song  
- Id, Title, Artist, Duration (en segundos)

### Subscription
- Id, UserId, PlanType (Free/Premium), IsActive, CreatedDate

### Playlist
- Id, UserId, Name, CreatedDate

### PlaylistSong
- Id, PlaylistId, SongId, AddedDate

## Tecnologías Utilizadas

- .NET Core 8
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI para documentación

## Datos de Prueba

La base de datos incluye datos de ejemplo:
- 3 usuarios (Juan Pérez, María García, Administrador)  
- 10 canciones populares
- Suscripciones activas
- Playlists de ejemplo con canciones

## Swagger UI

Una vez ejecutada la aplicación, puedes acceder a la documentación interactiva en:
`https://localhost:7000/swagger`