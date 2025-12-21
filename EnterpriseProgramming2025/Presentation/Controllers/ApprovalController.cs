using System;
using System.Linq;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseProgramming2025.Presentation.Filters;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    [Authorize]
    public class ApprovalController : Controller
    {
        private readonly ItemsRepository dbRepo;

        public ApprovalController([FromKeyedServices("db")] ItemsRepository dbRepo)
        {
            this.dbRepo = dbRepo;
        }

        // Admin sees pending restaurants
        [Authorize(Roles = "Admin")]
        public IActionResult Restaurants()
        {
            var savedRestaurants = dbRepo.GetRestaurants(null).ToList();
            var pending = dbRepo.GetRestaurants("Pending").ToList();
            return View(pending);
        }

        // Owner sees their pending menu items
        public IActionResult MenuItems()
        {
            var email = User.Identity?.Name ?? "";
            // only menu items whose restaurant owner email matches user
            var pending = dbRepo.GetMenuItems(null, "Pending")
                                .Where(m => m.Restaurant != null &&
                                            m.Restaurant.OwnerEmailAddress == email)
                                .ToList();
            return View(pending);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner")]
        [ServiceFilter(typeof(ApprovalFilterAttribute))]
        public IActionResult ApproveRestaurants(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return RedirectToAction("Restaurants");

            dbRepo.ApproveRestaurants(ids);

            return RedirectToAction("Restaurants");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ApprovedRestaurants()
        {
            var approved = dbRepo.GetRestaurants("Approved").ToList();
            return View(approved);
        }


        [HttpPost]
        [ServiceFilter(typeof(ApprovalFilterAttribute))]
        public IActionResult ApproveMenuItems(Guid[] ids)
        {
            if (ids == null || !ids.Any())
                return RedirectToAction("MenuItems");

            dbRepo.ApproveMenuItems(ids);
            return RedirectToAction("MenuItems");
        }

        public IActionResult ApprovedMenuItems()
        {
            var email = User.Identity?.Name ?? "";

            var approved = dbRepo.GetMenuItems(null, "Approved")
                .Where(m => m.Restaurant != null &&
                            m.Restaurant.OwnerEmailAddress == email)
                .ToList();

            return View(approved);
        }

    }
}
