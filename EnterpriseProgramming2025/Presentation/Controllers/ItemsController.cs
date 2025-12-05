using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemsRepository _dbRepo;

        public ItemsController([FromKeyedServices("db")] IItemsRepository dbRepo)
        {
            _dbRepo = dbRepo;
        }

        // Shows approved restaurants or menu items
        public IActionResult Catalog(int? restaurantId)
        {
            if (restaurantId == null)
            {
                // Load all approved restaurants
                var restaurants = _dbRepo.GetRestaurants(status: "Approved").ToList();
                return View(restaurants);
            }
            else
            {
                // Load approved menu items for one restaurant
                var menuItems = _dbRepo.GetMenuItems(restaurantId: restaurantId, status: "Approved").ToList();
                return View(menuItems);
            }
        }
    }
}