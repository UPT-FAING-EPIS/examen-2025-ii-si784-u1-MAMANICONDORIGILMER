using System.ComponentModel.DataAnnotations;

namespace MusicPlatformAPI.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Artist { get; set; } = string.Empty;

        [Required]
        public int Duration { get; set; } // Duración en segundos

        // Navegación
        public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    }
}