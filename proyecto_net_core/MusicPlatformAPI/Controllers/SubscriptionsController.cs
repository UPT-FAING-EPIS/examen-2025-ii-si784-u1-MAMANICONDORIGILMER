using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlatformAPI.Data;
using MusicPlatformAPI.Models;

namespace MusicPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly MusicDbContext _context;

        public SubscriptionsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: api/subscriptions/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<object>> GetUserSubscription(int userId)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.User)
                .Where(s => s.UserId == userId && s.IsActive)
                .Select(s => new {
                    s.Id,
                    s.UserId,
                    UserName = s.User.Name,
                    UserEmail = s.User.Email,
                    s.PlanType,
                    s.IsActive,
                    s.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return NotFound(new { message = "Suscripción activa no encontrada para este usuario" });
            }

            return Ok(subscription);
        }

        // POST: api/subscriptions
        [HttpPost]
        public async Task<ActionResult<object>> CreateOrRenewSubscription([FromBody] CreateSubscriptionRequest request)
        {
            // Verificar si el usuario existe
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Usuario no encontrado" });
            }

            // Verificar si hay una suscripción activa
            var existingSubscription = await _context.Subscriptions
                .Where(s => s.UserId == request.UserId && s.IsActive)
                .FirstOrDefaultAsync();

            if (existingSubscription != null)
            {
                // Actualizar la suscripción existente
                existingSubscription.PlanType = request.PlanType;
                existingSubscription.CreatedDate = DateTime.Now;
                _context.Subscriptions.Update(existingSubscription);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Suscripción renovada exitosamente",
                    subscriptionId = existingSubscription.Id,
                    planType = existingSubscription.PlanType
                });
            }
            else
            {
                // Crear nueva suscripción
                var newSubscription = new Subscription
                {
                    UserId = request.UserId,
                    PlanType = request.PlanType,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Subscriptions.Add(newSubscription);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Suscripción creada exitosamente",
                    subscriptionId = newSubscription.Id,
                    planType = newSubscription.PlanType
                });
            }
        }

        // DELETE: api/subscriptions/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelSubscription(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            
            if (subscription == null)
            {
                return NotFound(new { message = "Suscripción no encontrada" });
            }

            if (!subscription.IsActive)
            {
                return BadRequest(new { message = "La suscripción ya está cancelada" });
            }

            subscription.IsActive = false;
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Suscripción cancelada exitosamente" });
        }

        // GET: api/subscriptions/plans
        [HttpGet("plans")]
        public ActionResult<object> GetAvailablePlans()
        {
            var plans = new[]
            {
                new { 
                    Name = "Free", 
                    Description = "Acceso básico con publicidad", 
                    Price = 0.0,
                    Features = new[] { "Música básica", "Con publicidad", "Calidad estándar" }
                },
                new { 
                    Name = "Premium", 
                    Description = "Acceso completo sin publicidad", 
                    Price = 9.99,
                    Features = new[] { "Música sin límites", "Sin publicidad", "Calidad alta", "Descargas offline" }
                }
            };

            return Ok(plans);
        }
    }

    // DTO para crear suscripción
    public class CreateSubscriptionRequest
    {
        public int UserId { get; set; }
        public string PlanType { get; set; } = string.Empty; // "Free" o "Premium"
    }
}