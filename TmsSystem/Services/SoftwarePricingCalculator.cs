namespace TmsSystem.Services
{
    /// <summary>
    /// מחשבון תמחור תוכנה - מחשב את מחיר המכירה המומלץ על בסיס עלויות תפעול
    /// Software Pricing Calculator - Calculates recommended sale price based on operational costs
    /// </summary>
    public class SoftwarePricingCalculator
    {
        // עלויות תפעול חודשיות קבועות
        // Fixed monthly operational costs
        private const decimal ServerCostPerMonth = 6m;
        private const decimal EmailServerCostPerMonth = 8m;

        /// <summary>
        /// מחשב את עלויות התפעול החודשיות
        /// Calculates monthly operational costs
        /// </summary>
        public decimal GetMonthlyOperationalCost()
        {
            return ServerCostPerMonth + EmailServerCostPerMonth;
        }

        /// <summary>
        /// מחשב את עלויות התפעול השנתיות
        /// Calculates annual operational costs
        /// </summary>
        public decimal GetAnnualOperationalCost()
        {
            return GetMonthlyOperationalCost() * 12;
        }

        /// <summary>
        /// מחשב מחיר מכירה מומלץ על בסיס עלויות תפעול ומספר חודשים
        /// Calculates recommended sale price based on operational costs and number of months
        /// </summary>
        /// <param name="numberOfMonths">מספר חודשים של כיסוי עלויות (Number of months to cover costs)</param>
        /// <param name="profitMarginPercentage">אחוז רווח מעל עלויות (Profit margin percentage above costs)</param>
        public decimal CalculateRecommendedPrice(int numberOfMonths, decimal profitMarginPercentage = 0)
        {
            var totalOperationalCost = GetMonthlyOperationalCost() * numberOfMonths;
            var priceWithProfit = totalOperationalCost * (1 + profitMarginPercentage / 100);
            return Math.Round(priceWithProfit, 2);
        }

        /// <summary>
        /// מחזיר פירוט מלא של חישוב התמחור
        /// Returns detailed pricing calculation breakdown
        /// </summary>
        public PricingBreakdown GetPricingBreakdown(int numberOfMonths, decimal profitMarginPercentage = 0)
        {
            var monthlyOperationalCost = GetMonthlyOperationalCost();
            var totalOperationalCost = monthlyOperationalCost * numberOfMonths;
            var profitAmount = totalOperationalCost * (profitMarginPercentage / 100);
            var recommendedPrice = totalOperationalCost + profitAmount;

            return new PricingBreakdown
            {
                ServerCostPerMonth = ServerCostPerMonth,
                EmailServerCostPerMonth = EmailServerCostPerMonth,
                MonthlyOperationalCost = monthlyOperationalCost,
                NumberOfMonths = numberOfMonths,
                TotalOperationalCost = totalOperationalCost,
                ProfitMarginPercentage = profitMarginPercentage,
                ProfitAmount = Math.Round(profitAmount, 2),
                RecommendedPrice = Math.Round(recommendedPrice, 2)
            };
        }
    }

    /// <summary>
    /// מודל לפירוט חישוב התמחור
    /// Model for pricing calculation breakdown
    /// </summary>
    public class PricingBreakdown
    {
        public decimal ServerCostPerMonth { get; set; }
        public decimal EmailServerCostPerMonth { get; set; }
        public decimal MonthlyOperationalCost { get; set; }
        public int NumberOfMonths { get; set; }
        public decimal TotalOperationalCost { get; set; }
        public decimal ProfitMarginPercentage { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal RecommendedPrice { get; set; }
    }
}
