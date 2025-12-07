using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IItemsRepository _dbRepo;

        public ApprovalController([FromKeyedServices("db")] IItemsRepository dbRepo)
        {
            _dbRepo = dbRepo;
        }

        // List restaurants that are pending approval
        public IActionResult Restaurants()
        {
            var pending = _dbRepo.GetRestaurants(status: "Pending").ToList();
            return View(pending);
        }

        [HttpPost]
        public IActionResult ApproveRestaurants(int[] ids)
        {
            _dbRepo.ApproveRestaurants(ids);
            return RedirectToAction("Restaurants");
        }

        // List menu items that are pending approval
        public IActionResult MenuItems()
        {
            var pending = _dbRepo.GetMenuItems(status: "Pending").ToList();
            return View(pending);
        }

        [HttpPost]
        public IActionResult ApproveMenuItems(Guid[] ids)
        {
            _dbRepo.ApproveMenuItems(ids);
            return RedirectToAction("MenuItems");
        }
    }
}
