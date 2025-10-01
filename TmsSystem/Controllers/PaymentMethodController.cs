using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TmsSystem.Controllers
{
    public class PaymentMethodController : Controller
    {
        // GET: PaymentMethodController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PaymentMethodController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PaymentMethodController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymentMethodController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PaymentMethodController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaymentMethodController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }




    // GET: PaymentMethodController/Delete/5
    public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PaymentMethodController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
