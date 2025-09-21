# Script de Pruebas de la API Music Platform
# Ejecutar desde PowerShell: .\test-api.ps1

$baseUrl = "http://localhost:5248"
$headers = @{ "Content-Type" = "application/json" }

function Write-TestHeader($title) {
    Write-Host "`n=== $title ===" -ForegroundColor Cyan
}

function Test-Endpoint($name, $method, $url, $body = $null) {
    try {
        Write-Host "`n🧪 Probando: $name" -ForegroundColor Yellow
        Write-Host "   $method $url" -ForegroundColor Gray
        
        if ($body) {
            $response = Invoke-RestMethod -Uri $url -Method $method -Headers $headers -Body $body
        } else {
            $response = Invoke-RestMethod -Uri $url -Method $method -Headers $headers
        }
        
        Write-Host "✅ ÉXITO" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 4
        return $response
    } catch {
        Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            Write-Host "   Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
        return $null
    }
}

# ================================
# INICIO DE PRUEBAS
# ================================

Write-Host "🎵 INICIANDO PRUEBAS DE LA API MUSIC PLATFORM 🎵" -ForegroundColor Magenta
Write-Host "Base URL: $baseUrl" -ForegroundColor Gray

# ================================
# 1. ENDPOINTS DE SALUD
# ================================
Write-TestHeader "1. HEALTH CHECKS"

Test-Endpoint "Health Check Básico" "GET" "$baseUrl/api/health"
Test-Endpoint "Health Check Base de Datos" "GET" "$baseUrl/api/health/database"

# ================================
# 2. ENDPOINTS DE MÚSICA
# ================================
Write-TestHeader "2. MUSIC ENDPOINTS"

Test-Endpoint "Listar todas las canciones" "GET" "$baseUrl/api/music"
Test-Endpoint "Obtener canción específica (ID: 1)" "GET" "$baseUrl/api/music/1"
Test-Endpoint "Obtener canción específica (ID: 5)" "GET" "$baseUrl/api/music/5"
Test-Endpoint "Buscar por artista 'Queen'" "GET" "$baseUrl/api/music/search?artist=Queen"
Test-Endpoint "Buscar por título 'Imagine'" "GET" "$baseUrl/api/music/search?title=Imagine"
Test-Endpoint "Buscar canción inexistente (ID: 999)" "GET" "$baseUrl/api/music/999"

# ================================
# 3. ENDPOINTS DE SUSCRIPCIONES
# ================================
Write-TestHeader "3. SUBSCRIPTION ENDPOINTS"

Test-Endpoint "Ver planes disponibles" "GET" "$baseUrl/api/subscriptions/plans"
Test-Endpoint "Ver suscripción usuario 1" "GET" "$baseUrl/api/subscriptions/1"
Test-Endpoint "Ver suscripción usuario 2" "GET" "$baseUrl/api/subscriptions/2"

# Crear nueva suscripción Premium para usuario 2
$newSubscription = @{
    userId = 2
    planType = "Premium"
} | ConvertTo-Json

Test-Endpoint "Crear suscripción Premium usuario 2" "POST" "$baseUrl/api/subscriptions" $newSubscription

# ================================
# 4. ENDPOINTS DE PLAYLISTS
# ================================
Write-TestHeader "4. PLAYLIST ENDPOINTS"

Test-Endpoint "Listar playlists usuario 1" "GET" "$baseUrl/api/playlists/1"
Test-Endpoint "Listar playlists usuario 2" "GET" "$baseUrl/api/playlists/2"

# Crear nueva playlist
$newPlaylist = @{
    userId = 1
    name = "Mi Playlist de Prueba PowerShell"
} | ConvertTo-Json

$playlistResponse = Test-Endpoint "Crear nueva playlist" "POST" "$baseUrl/api/playlists" $newPlaylist

if ($playlistResponse -and $playlistResponse.playlistId) {
    $playlistId = $playlistResponse.playlistId
    
    # Agregar canciones a la playlist
    $addSong1 = @{ songId = 3 } | ConvertTo-Json
    $addSong2 = @{ songId = 7 } | ConvertTo-Json
    
    Test-Endpoint "Agregar canción 3 a playlist" "POST" "$baseUrl/api/playlists/$playlistId/songs" $addSong1
    Test-Endpoint "Agregar canción 7 a playlist" "POST" "$baseUrl/api/playlists/$playlistId/songs" $addSong2
    
    Test-Endpoint "Ver detalles de la playlist creada" "GET" "$baseUrl/api/playlists/$playlistId/details"
    
    # Remover una canción
    Test-Endpoint "Remover canción 3 de playlist" "DELETE" "$baseUrl/api/playlists/$playlistId/songs/3"
    
    Test-Endpoint "Ver playlist después de remover canción" "GET" "$baseUrl/api/playlists/$playlistId/details"
}

# ================================
# 5. ENDPOINTS DE REPORTES
# ================================
Write-TestHeader "5. REPORTS ENDPOINTS"

Test-Endpoint "Reporte de suscripciones" "POST" "$baseUrl/api/reports/subscriptions"
Test-Endpoint "Reporte de uso" "POST" "$baseUrl/api/reports/usage"
Test-Endpoint "Reporte de usuarios" "POST" "$baseUrl/api/reports/users"

# ================================
# PRUEBAS DE CASOS EDGE
# ================================
Write-TestHeader "6. CASOS EDGE Y ERRORES"

Test-Endpoint "Usuario inexistente (ID: 999)" "GET" "$baseUrl/api/subscriptions/999"
Test-Endpoint "Playlist inexistente (ID: 999)" "GET" "$baseUrl/api/playlists/999/details"

# Datos inválidos
$invalidSubscription = @{
    userId = 999
    planType = "InvalidPlan"
} | ConvertTo-Json

Test-Endpoint "Crear suscripción con datos inválidos" "POST" "$baseUrl/api/subscriptions" $invalidSubscription

Write-Host "`n🎉 PRUEBAS COMPLETADAS 🎉" -ForegroundColor Magenta
Write-Host "Revisa los resultados arriba para verificar el funcionamiento de la API" -ForegroundColor Gray