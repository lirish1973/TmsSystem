using Microsoft.AspNetCore.Mvc;

namespace TmsSystem.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            DebugAlert("בדיקה – ערך משתנה XYZ");
            return View();
        }
        protected void DebugAlert(string message)
        {
            TempData["DebugAlertMessage"] = message;
        }
    }
}
