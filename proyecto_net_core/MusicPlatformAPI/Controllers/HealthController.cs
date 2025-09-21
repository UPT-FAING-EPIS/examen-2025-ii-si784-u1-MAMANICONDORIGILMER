using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlatformAPI.Data;

namespace MusicPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly MusicDbContext _context;

        public HealthController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: api/health
        [HttpGet]
        public ActionResult GetHealth()
        {
            return Ok(new { 
                status = "API is running", 
                timestamp = DateTime.Now,
                message = "Music Platform API is healthy"
            });
        }

        // GET: api/health/database
        [HttpGet("database")]
        public async Task<ActionResult> CheckDatabase()
        {
            try
            {
                // Intentar conectar a la base de datos
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    // Contar usuarios para probar que la conexión funciona
                    var userCount = await _context.Users.CountAsync();
                    var songCount = await _context.Songs.CountAsync();
                    
                    return Ok(new { 
                        status = "Connected", 
                        database = "MusicPlatformSimpleDB",
                        userCount = userCount,
                        songCount = songCount,
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    return StatusCode(503, new { 
                        status = "Cannot connect to database", 
                        timestamp = DateTime.Now 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { 
                    status = "Database error", 
                    error = ex.Message,
                    timestamp = DateTime.Now 
                });
            }
        }
    }
}