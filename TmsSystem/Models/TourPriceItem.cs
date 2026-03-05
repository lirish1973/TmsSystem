using System.ComponentModel.DataAnnotations;

namespace TmsSystem.Models
{
    public class TourPriceItem
    {
        [Key]
        public int Id { get; set; }

        public int TourId { get; set; }

        /// <summary>
        /// שם הקטגוריה - למשל: "זוג", "4 אנשים", "ילד", "תינוק", "קבוצה גדולה"
        /// </summary>
        [Required]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// מחיר לקטגוריה זו
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        // Navigation
        public Tour Tour { get; set; } = null!;
    }
}
