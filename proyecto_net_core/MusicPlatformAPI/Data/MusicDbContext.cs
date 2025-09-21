using Microsoft.EntityFrameworkCore;
using MusicPlatformAPI.Models;

namespace MusicPlatformAPI.Data
{
    public class MusicDbContext : DbContext
    {
        public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar indices únicos
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configurar restricciones de PlanType
            modelBuilder.Entity<Subscription>()
                .Property(s => s.PlanType)
                .HasConversion<string>();

            // Configurar valores por defecto
            modelBuilder.Entity<Subscription>()
                .Property(s => s.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Subscription>()
                .Property(s => s.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Playlist>()
                .Property(p => p.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PlaylistSong>()
                .Property(ps => ps.AddedDate)
                .HasDefaultValueSql("GETDATE()");

            base.OnModelCreating(modelBuilder);
        }
    }
}