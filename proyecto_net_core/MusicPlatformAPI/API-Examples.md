# Ejemplos de Llamadas a la API

## Ejemplos usando curl

### 1. Listar todas las canciones
```bash
curl -X GET "https://localhost:7000/api/music" -H "accept: application/json"
```

### 2. Obtener canción específica
```bash
curl -X GET "https://localhost:7000/api/music/1" -H "accept: application/json"
```

### 3. Buscar canciones por artista
```bash
curl -X GET "https://localhost:7000/api/music/search?artist=Queen" -H "accept: application/json"
```

### 4. Ver suscripción de usuario
```bash
curl -X GET "https://localhost:7000/api/subscriptions/1" -H "accept: application/json"
```

### 5. Crear suscripción Premium
```bash
curl -X POST "https://localhost:7000/api/subscriptions" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 2,
    "planType": "Premium"
  }'
```

### 6. Ver planes disponibles
```bash
curl -X GET "https://localhost:7000/api/subscriptions/plans" -H "accept: application/json"
```

### 7. Listar playlists de usuario
```bash
curl -X GET "https://localhost:7000/api/playlists/1" -H "accept: application/json"
```

### 8. Crear nueva playlist
```bash
curl -X POST "https://localhost:7000/api/playlists" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "name": "Mi Nueva Playlist"
  }'
```

### 9. Agregar canción a playlist
```bash
curl -X POST "https://localhost:7000/api/playlists/1/songs" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "songId": 5
  }'
```

### 10. Ver detalles de playlist
```bash
curl -X GET "https://localhost:7000/api/playlists/1/details" -H "accept: application/json"
```

### 11. Generar reporte de suscripciones
```bash
curl -X POST "https://localhost:7000/api/reports/subscriptions" \
  -H "accept: application/json" \
  -H "Content-Type: application/json"
```

### 12. Generar reporte de uso
```bash
curl -X POST "https://localhost:7000/api/reports/usage" \
  -H "accept: application/json" \
  -H "Content-Type: application/json"
```

### 13. Cancelar suscripción
```bash
curl -X DELETE "https://localhost:7000/api/subscriptions/1" -H "accept: application/json"
```

### 14. Remover canción de playlist
```bash
curl -X DELETE "https://localhost:7000/api/playlists/1/songs/3" -H "accept: application/json"
```

## Respuestas Esperadas

### Listar canciones
```json
[
  {
    "id": 1,
    "title": "Bohemian Rhapsody",
    "artist": "Queen",
    "duration": "5:55"
  },
  {
    "id": 2,
    "title": "Imagine",
    "artist": "John Lennon", 
    "duration": "3:03"
  }
]
```

### Ver suscripción
```json
{
  "id": 1,
  "userId": 1,
  "userName": "Juan Pérez",
  "userEmail": "usuario1@email.com",
  "planType": "Premium",
  "isActive": true,
  "createdDate": "2024-01-15T10:30:00"
}
```

### Playlists de usuario
```json
[
  {
    "id": 1,
    "name": "Mis Favoritas",
    "createdDate": "2024-01-15T10:30:00",
    "songCount": 3,
    "songs": [
      {
        "id": 1,
        "title": "Bohemian Rhapsody",
        "artist": "Queen",
        "duration": "5:55",
        "addedDate": "2024-01-15T11:00:00"
      }
    ]
  }
]
```

## Códigos de Estado HTTP

- **200 OK**: Operación exitosa
- **201 Created**: Recurso creado exitosamente
- **400 Bad Request**: Error en los datos enviados
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error del servidor

## Notas Importantes

1. La API usa HTTPS por defecto en desarrollo
2. Los IDs deben ser números enteros válidos
3. Los planes de suscripción son "Free" o "Premium" (case-sensitive)
4. Las fechas se devuelven en formato ISO 8601
5. La duración de las canciones se muestra en formato MM:SS para facilitar la lectura