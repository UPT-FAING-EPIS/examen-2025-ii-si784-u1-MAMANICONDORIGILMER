using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlatformAPI.Data;

namespace MusicPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly MusicDbContext _context;

        public ReportsController(MusicDbContext context)
        {
            _context = context;
        }

        // POST: api/reports/subscriptions
        [HttpPost("subscriptions")]
        public async Task<ActionResult<object>> GetSubscriptionReport()
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeSubscriptions = await _context.Subscriptions
                .Where(s => s.IsActive)
                .CountAsync();
            
            var subscriptionsByPlan = await _context.Subscriptions
                .Where(s => s.IsActive)
                .GroupBy(s => s.PlanType)
                .Select(g => new {
                    PlanType = g.Key,
                    Count = g.Count(),
                    Percentage = Math.Round((double)g.Count() / activeSubscriptions * 100, 2)
                })
                .ToListAsync();

            var recentSubscriptions = await _context.Subscriptions
                .Where(s => s.CreatedDate >= DateTime.Now.AddDays(-30))
                .GroupBy(s => s.CreatedDate.Date)
                .Select(g => new {
                    Date = g.Key,
                    NewSubscriptions = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var report = new {
                GeneratedAt = DateTime.Now,
                Summary = new {
                    TotalUsers = totalUsers,
                    ActiveSubscriptions = activeSubscriptions,
                    SubscriptionRate = Math.Round((double)activeSubscriptions / totalUsers * 100, 2)
                },
                SubscriptionsByPlan = subscriptionsByPlan,
                RecentActivity = recentSubscriptions
            };

            return Ok(report);
        }

        // POST: api/reports/usage
        [HttpPost("usage")]
        public async Task<ActionResult<object>> GetUsageReport()
        {
            var totalSongs = await _context.Songs.CountAsync();
            var totalPlaylists = await _context.Playlists.CountAsync();
            var averagePlaylistSize = await _context.PlaylistSongs
                .GroupBy(ps => ps.PlaylistId)
                .Select(g => g.Count())
                .DefaultIfEmpty(0)
                .AverageAsync();

            var topArtists = await _context.PlaylistSongs
                .Include(ps => ps.Song)
                .GroupBy(ps => ps.Song.Artist)
                .Select(g => new {
                    Artist = g.Key,
                    TimesInPlaylists = g.Count()
                })
                .OrderByDescending(x => x.TimesInPlaylists)
                .Take(10)
                .ToListAsync();

            var popularSongs = await _context.PlaylistSongs
                .Include(ps => ps.Song)
                .GroupBy(ps => ps.Song)
                .Select(g => new {
                    SongId = g.Key.Id,
                    Title = g.Key.Title,
                    Artist = g.Key.Artist,
                    TimesInPlaylists = g.Count()
                })
                .OrderByDescending(x => x.TimesInPlaylists)
                .Take(10)
                .ToListAsync();

            var playlistActivity = await _context.Playlists
                .Where(p => p.CreatedDate >= DateTime.Now.AddDays(-30))
                .GroupBy(p => p.CreatedDate.Date)
                .Select(g => new {
                    Date = g.Key,
                    NewPlaylists = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var report = new {
                GeneratedAt = DateTime.Now,
                Summary = new {
                    TotalSongs = totalSongs,
                    TotalPlaylists = totalPlaylists,
                    AveragePlaylistSize = Math.Round(averagePlaylistSize, 1)
                },
                TopArtists = topArtists,
                PopularSongs = popularSongs,
                RecentPlaylistActivity = playlistActivity
            };

            return Ok(report);
        }

        // POST: api/reports/users
        [HttpPost("users")]
        public async Task<ActionResult<object>> GetUserActivityReport()
        {
            var userStats = await _context.Users
                .Include(u => u.Subscriptions.Where(s => s.IsActive))
                .Include(u => u.Playlists)
                .Select(u => new {
                    u.Id,
                    u.Name,
                    u.Email,
                    HasActiveSubscription = u.Subscriptions.Any(s => s.IsActive),
                    PlanType = u.Subscriptions.Where(s => s.IsActive).Select(s => s.PlanType).FirstOrDefault(),
                    PlaylistCount = u.Playlists.Count(),
                    TotalSongsInPlaylists = u.Playlists.SelectMany(p => p.PlaylistSongs).Count(),
                    LastActivity = u.Playlists.OrderByDescending(p => p.CreatedDate).Select(p => p.CreatedDate).FirstOrDefault()
                })
                .ToListAsync();

            var activeUsers = userStats.Where(u => u.LastActivity >= DateTime.Now.AddDays(-30)).Count();
            var premiumUsers = userStats.Where(u => u.PlanType == "Premium").Count();

            var report = new {
                GeneratedAt = DateTime.Now,
                Summary = new {
                    TotalUsers = userStats.Count,
                    ActiveUsers = activeUsers,
                    PremiumUsers = premiumUsers,
                    ActivityRate = Math.Round((double)activeUsers / userStats.Count * 100, 2)
                },
                UserDetails = userStats.OrderByDescending(u => u.LastActivity).Take(20)
            };

            return Ok(report);
        }
    }
}