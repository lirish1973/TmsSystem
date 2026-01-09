using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("imagegallery")]
    public class ImageGallery
    {
        [Key]
        [Column("ImageId")]
        public int ImageId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("FileName")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("FilePath")]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("UploadedAt")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column("FileSize")]
        public long FileSize { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // For tracking usage
        [Column("UsageCount")]
        public int UsageCount { get; set; } = 0;
    }
}
