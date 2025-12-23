using Microsoft.AspNetCore.Mvc;
using TmsSystem.Services;
using TmsSystem.ViewModels;

namespace TmsSystem.Controllers
{
    /// <summary>
    /// קונטרולר למחשבון תמחור תוכנה
    /// Controller for Software Pricing Calculator
    /// </summary>
    public class PricingController : Controller
    {
        private readonly SoftwarePricingCalculator _pricingCalculator;

        public PricingController()
        {
            _pricingCalculator = new SoftwarePricingCalculator();
        }

        /// <summary>
        /// מציג את מחשבון התמחור
        /// Displays the pricing calculator
        /// </summary>
        [HttpGet]
        public IActionResult Calculator()
        {
            var model = new SoftwarePricingViewModel
            {
                NumberOfMonths = 12,
                ProfitMarginPercentage = 50,
                ServerCostPerMonth = 6,
                EmailServerCostPerMonth = 8,
                MonthlyOperationalCost = 14
            };

            return View(model);
        }

        /// <summary>
        /// מחשב את התמחור המומלץ
        /// Calculates the recommended pricing
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculator(SoftwarePricingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var breakdown = _pricingCalculator.GetPricingBreakdown(
                model.NumberOfMonths,
                model.ProfitMarginPercentage
            );

            model.ServerCostPerMonth = breakdown.ServerCostPerMonth;
            model.EmailServerCostPerMonth = breakdown.EmailServerCostPerMonth;
            model.MonthlyOperationalCost = breakdown.MonthlyOperationalCost;
            model.TotalOperationalCost = breakdown.TotalOperationalCost;
            model.ProfitAmount = breakdown.ProfitAmount;
            model.RecommendedPrice = breakdown.RecommendedPrice;

            return View(model);
        }
    }
}
