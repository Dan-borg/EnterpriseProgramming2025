using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Restaurants", "Approval");

            return RedirectToAction("Catalog", "Items");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
