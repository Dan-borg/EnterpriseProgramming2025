using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ItemsRepository dbRepo;

        public ItemsController([FromKeyedServices("db")] ItemsRepository dbRepo)
        {
            this.dbRepo = dbRepo;
        }

        public IActionResult Catalog(int? restaurantId, string view = "card")
        {
            ViewBag.ViewMode = view;

            if (restaurantId == null)
            {
                var restaurants = dbRepo.GetRestaurants("Approved")
                    .Cast<Domain.Interfaces.ItemValidating>()
                    .ToList();
                return View(restaurants);
            }
            else
            {
                var menuItems = dbRepo.GetMenuItems(restaurantId.Value, "Approved")
                    .Cast<Domain.Interfaces.ItemValidating>()
                    .ToList();
                return View(menuItems);
            }
        }
    }
}
