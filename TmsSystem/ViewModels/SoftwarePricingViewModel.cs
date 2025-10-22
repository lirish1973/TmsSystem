using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    /// <summary>
    /// מודל עבור מחשבון תמחור תוכנה
    /// ViewModel for Software Pricing Calculator
    /// </summary>
    public class SoftwarePricingViewModel
    {
        [Required(ErrorMessage = "יש להזין מספר חודשים")]
        [Range(1, 120, ErrorMessage = "מספר החודשים חייב להיות בין 1 ל-120")]
        [Display(Name = "מספר חודשים לכיסוי")]
        public int NumberOfMonths { get; set; } = 12;

        [Range(0, 1000, ErrorMessage = "אחוז הרווח חייב להיות בין 0 ל-1000")]
        [Display(Name = "אחוז רווח (%)")]
        public decimal ProfitMarginPercentage { get; set; } = 50;

        // תוצאות החישוב - ימולאו על ידי הקונטרולר
        // Calculation results - will be filled by the controller
        public decimal ServerCostPerMonth { get; set; }
        public decimal EmailServerCostPerMonth { get; set; }
        public decimal MonthlyOperationalCost { get; set; }
        public decimal TotalOperationalCost { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal RecommendedPrice { get; set; }
    }
}
