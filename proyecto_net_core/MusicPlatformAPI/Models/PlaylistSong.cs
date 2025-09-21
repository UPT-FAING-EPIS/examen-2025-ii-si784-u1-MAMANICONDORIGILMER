using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatformAPI.Models
{
    public class PlaylistSong
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlaylistId { get; set; }

        [Required]
        public int SongId { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("PlaylistId")]
        public virtual Playlist Playlist { get; set; } = null!;

        [ForeignKey("SongId")]
        public virtual Song Song { get; set; } = null!;
    }
}