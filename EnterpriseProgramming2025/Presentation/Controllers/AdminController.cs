using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    private readonly ItemsRepository _db;

    public AdminController([FromKeyedServices("db")] ItemsRepository db)
    {
        _db = db;
    }

    // GET: Admin/CreateRestaurant
    public IActionResult CreateRestaurant()
    {
        return View();
    }

    // POST: Admin/CreateRestaurant
    [HttpPost]
    public IActionResult CreateRestaurant(Restaurant model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.Status = "Pending"; // Admin still approves later

        _db.SaveRestaurants(new List<Restaurant> { model });

        return RedirectToAction("Restaurants", "Approval");
    }
}
