using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlatformAPI.Data;
using MusicPlatformAPI.Models;

namespace MusicPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicController : ControllerBase
    {
        private readonly MusicDbContext _context;

        public MusicController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: api/music
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMusic()
        {
            var songs = await _context.Songs
                .Select(s => new {
                    s.Id,
                    s.Title,
                    s.Artist,
                    Duration = $"{s.Duration / 60}:{s.Duration % 60:D2}" // Formato MM:SS
                })
                .ToListAsync();

            return Ok(songs);
        }

        // GET: api/music/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMusic(int id)
        {
            var song = await _context.Songs
                .Where(s => s.Id == id)
                .Select(s => new {
                    s.Id,
                    s.Title,
                    s.Artist,
                    Duration = $"{s.Duration / 60}:{s.Duration % 60:D2}", // Formato MM:SS
                    DurationInSeconds = s.Duration
                })
                .FirstOrDefaultAsync();

            if (song == null)
            {
                return NotFound(new { message = "Canción no encontrada" });
            }

            return Ok(song);
        }

        // GET: api/music/search?artist={artist}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchMusic([FromQuery] string? artist, [FromQuery] string? title)
        {
            var query = _context.Songs.AsQueryable();

            if (!string.IsNullOrEmpty(artist))
            {
                query = query.Where(s => s.Artist.Contains(artist));
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(s => s.Title.Contains(title));
            }

            var songs = await query
                .Select(s => new {
                    s.Id,
                    s.Title,
                    s.Artist,
                    Duration = $"{s.Duration / 60}:{s.Duration % 60:D2}"
                })
                .ToListAsync();

            return Ok(songs);
        }
    }
}