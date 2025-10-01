using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;
using static TmsSystem.ViewModels.CreateOfferViewModel;

namespace TmsSystem.Controllers
{
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            var model = new CreateOfferViewModel
            {
                Guides = await _context.Guides
                    .Select(g => new GuideSelectViewModel
                    {
                        GuideId = g.GuideId,
                        GuideName = g.GuideName ?? string.Empty
                    })
                    .ToListAsync(),

                Customers = await _context.Customers
                    .Select(c => new CustomerSelectViewModel
                    {
                        CustomerId = c.CustomerId,
                        DisplayName = (c.FullName ?? c.CustomerName ?? string.Empty) + $" ({c.Phone})"
                    })
                    .ToListAsync(),

                PaymentMethods = await _context.PaymentsMethod
                    .Select(pm => new PaymentMethodSelectViewModel
                    {
                        PaymentMethodId = pm.id,      // שם השדה במודל: Id
                        PaymentName = pm.method       // שם השדה במודל: Method
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Include(o => o.Customer)
                .Include(o => o.GuideName)
                .Include(o => o.Tour)
                .ToListAsync();

            return View(offers);
        }

        // POST: Offers/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // טעינה מחדש של רשימות הבחירה אם יש שגיאה
                model.Customers = await _context.Customers
                    .Select(c => new CustomerSelectViewModel
                    {
                        CustomerId = c.CustomerId,
                        DisplayName = (c.FullName ?? c.CustomerName ?? string.Empty) + $" ({c.Phone})"
                    })
                    .ToListAsync();

                model.Guides = await _context.Guides
                    .Select(g => new GuideSelectViewModel
                    {
                        GuideId = g.GuideId,
                        GuideName = g.GuideName ?? string.Empty
                    })
                    .ToListAsync();

                model.PaymentMethods = await _context.PaymentsMethod
         .Select(pm => new PaymentMethodSelectViewModel
         {
             PaymentMethodId = pm.id,      // שם השדה במודל: Id
             PaymentName = pm.method       // שם השדה במודל: Method
         })
         .ToListAsync();

                return View(model);
            }

            var offer = new Offer
            {
                CustomerId = model.CustomerId,
                Participants = model.Participants,
                TourDate = model.TourDate,
                PickupLocation = model.PickupLocation,
                Price = model.Price,
                PriceIncludes = model.PriceIncludes,
                PriceExcludes = model.PriceExcludes,
                GuideId = model.GuideId,
                TotalPayment = model.TotalPayment,
                SpecialRequests = model.SpecialRequests,
                LunchIncluded = model.LunchIncluded,
                PaymentId = model.PaymentMethodId, // כאן משתמשים ב-ID מה PaymentsMethod!
                TourId = model.TourId,
                CreatedAt = DateTime.Now
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Offers");
        }
    }
}