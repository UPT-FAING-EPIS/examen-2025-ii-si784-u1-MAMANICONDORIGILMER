using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlatformAPI.Data;
using MusicPlatformAPI.Models;

namespace MusicPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly MusicDbContext _context;

        public PlaylistsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: api/playlists/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserPlaylists(int userId)
        {
            var playlists = await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .Where(p => p.UserId == userId)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.CreatedDate,
                    SongCount = p.PlaylistSongs.Count,
                    Songs = p.PlaylistSongs.Select(ps => new {
                        ps.Song.Id,
                        ps.Song.Title,
                        ps.Song.Artist,
                        Duration = $"{ps.Song.Duration / 60}:{ps.Song.Duration % 60:D2}",
                        ps.AddedDate
                    }).ToList()
                })
                .ToListAsync();

            return Ok(playlists);
        }

        // POST: api/playlists
        [HttpPost]
        public async Task<ActionResult<object>> CreatePlaylist([FromBody] CreatePlaylistRequest request)
        {
            // Verificar si el usuario existe
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Usuario no encontrado" });
            }

            var playlist = new Playlist
            {
                UserId = request.UserId,
                Name = request.Name,
                CreatedDate = DateTime.Now
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Playlist creada exitosamente",
                playlistId = playlist.Id,
                name = playlist.Name,
                createdDate = playlist.CreatedDate
            });
        }

        // POST: api/playlists/{playlistId}/songs
        [HttpPost("{playlistId}/songs")]
        public async Task<ActionResult> AddSongToPlaylist(int playlistId, [FromBody] AddSongRequest request)
        {
            // Verificar si la playlist existe
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null)
            {
                return NotFound(new { message = "Playlist no encontrada" });
            }

            // Verificar si la canción existe
            var song = await _context.Songs.FindAsync(request.SongId);
            if (song == null)
            {
                return BadRequest(new { message = "Canción no encontrada" });
            }

            // Verificar si la canción ya está en la playlist
            var existingSong = await _context.PlaylistSongs
                .Where(ps => ps.PlaylistId == playlistId && ps.SongId == request.SongId)
                .FirstOrDefaultAsync();

            if (existingSong != null)
            {
                return BadRequest(new { message = "La canción ya existe en la playlist" });
            }

            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = request.SongId,
                AddedDate = DateTime.Now
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Canción agregada exitosamente a la playlist" });
        }

        // DELETE: api/playlists/{playlistId}/songs/{songId}
        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<ActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var playlistSong = await _context.PlaylistSongs
                .Where(ps => ps.PlaylistId == playlistId && ps.SongId == songId)
                .FirstOrDefaultAsync();

            if (playlistSong == null)
            {
                return NotFound(new { message = "Canción no encontrada en la playlist" });
            }

            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Canción removida exitosamente de la playlist" });
        }

        // GET: api/playlists/{id}/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<object>> GetPlaylistDetails(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .Where(p => p.Id == id)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.CreatedDate,
                    Owner = new {
                        p.User.Id,
                        p.User.Name,
                        p.User.Email
                    },
                    SongCount = p.PlaylistSongs.Count,
                    TotalDuration = p.PlaylistSongs.Sum(ps => ps.Song.Duration),
                    Songs = p.PlaylistSongs.Select(ps => new {
                        ps.Song.Id,
                        ps.Song.Title,
                        ps.Song.Artist,
                        Duration = $"{ps.Song.Duration / 60}:{ps.Song.Duration % 60:D2}",
                        ps.AddedDate
                    }).OrderBy(s => s.AddedDate).ToList()
                })
                .FirstOrDefaultAsync();

            if (playlist == null)
            {
                return NotFound(new { message = "Playlist no encontrada" });
            }

            return Ok(playlist);
        }
    }

    // DTOs
    public class CreatePlaylistRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AddSongRequest
    {
        public int SongId { get; set; }
    }
}