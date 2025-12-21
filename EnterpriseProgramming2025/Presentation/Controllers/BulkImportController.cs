using System.IO.Compression;
using Domain.Interfaces;
using Domain.Models;
using EnterpriseProgramming2025.Presentation.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EnterpriseProgramming2025.Data;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BulkImportController : Controller
    {
        private readonly ImportItemFactory factory;
        private readonly ItemsRepository memoryRepo;
        private readonly ItemsRepository dbRepo;
        private readonly IWebHostEnvironment env;

        public BulkImportController(
            ImportItemFactory factory,
            [FromKeyedServices("memory")] ItemsRepository memoryRepo,
            [FromKeyedServices("db")] ItemsRepository dbRepo,
            IWebHostEnvironment env)
        {
            this.factory = factory;
            this.memoryRepo = memoryRepo;
            this.dbRepo = dbRepo;
            this.env = env;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Preview(string jsonInput)
        {
            ViewBag.JsonInput = jsonInput;

            var items = factory.Create(jsonInput);
            if (!items.Any())
            {
                ViewBag.Error = "No valid items found.";
                return View("Index");
            }

            var restaurants = items.OfType<Restaurant>().ToList();
            var menuItems = items.OfType<MenuItem>().ToList();

            memoryRepo.Clear();
            memoryRepo.SaveRestaurants(restaurants);
            memoryRepo.SaveMenuItems(menuItems);

            ViewBag.Items = items;
            ViewBag.RestaurantCount = restaurants.Count;
            ViewBag.MenuItemCount = menuItems.Count;

            return View("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult DownloadZip()
        {
            var restaurants = memoryRepo.GetRestaurants().ToList();
            var menuItems = memoryRepo.GetMenuItems().ToList();

            if (!restaurants.Any() && !menuItems.Any())
                return BadRequest("No items in memory.");

            using var ms = new MemoryStream();

            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var r in restaurants)
                {
                    var entry = zip.CreateEntry($"item-{r.Id}/default.jpg");
                    using var stream = entry.Open();
                    stream.WriteByte(0); 
                }

                foreach (var m in menuItems)
                {
                    var entry = zip.CreateEntry($"item-{m.Id}/default.jpg");
                    using var stream = entry.Open();
                    stream.WriteByte(0);
                }
            }

            ms.Position = 0; 

            return File(ms.ToArray(), "application/zip", "items.zip");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Commit(IFormFile zipFile)
        {
            var restaurants = memoryRepo.GetRestaurants().ToList();
            var menuItems = memoryRepo.GetMenuItems().ToList();

            if (!restaurants.Any() && !menuItems.Any())
                return BadRequest("Nothing to commit.");

            var imagesDir = Path.Combine(env.WebRootPath, "images");
            Directory.CreateDirectory(imagesDir);

            using (var zip = new ZipArchive(zipFile.OpenReadStream()))
            {
                foreach (var entry in zip.Entries.Where(e => e.Name == "default.jpg"))
                {
                    var id = Path.GetDirectoryName(entry.FullName)?.Replace("item-", "");
                    var imgName = $"{Guid.NewGuid()}.jpg";
                    var fullPath = Path.Combine(imagesDir, imgName);
                    entry.ExtractToFile(fullPath, true);

                    var rel = $"images/{imgName}";

                    if (int.TryParse(id, out var rid))
                        restaurants.First(r => r.Id == rid).ImagePath = rel;

                    if (Guid.TryParse(id, out var mid))
                        menuItems.First(m => m.Id == mid).ImagePath = rel;
                }
            }

            // 1. Save restaurants FIRST
            dbRepo.SaveRestaurants(restaurants);

            // 2. Reload saved restaurants WITH tracking
            using var scope = HttpContext.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var savedRestaurants = dbContext.Restaurants.ToList();

            // 3. Attach Restaurant entity to each MenuItem
            
                foreach (var m in menuItems)
                {
                    if (m.Restaurant == null)
                        continue;

                    var ownerEmail = m.Restaurant.OwnerEmailAddress;

                    var dbRestaurant = savedRestaurants
                        .FirstOrDefault(r => r.OwnerEmailAddress == ownerEmail);

                    if (dbRestaurant == null)
                        continue;

                    m.RestaurantId = dbRestaurant.Id;
                }
            

            // 4. Save menu items
            dbRepo.SaveMenuItems(menuItems);

            // 5. Clear memory
            memoryRepo.Clear();


            return RedirectToAction("Restaurants", "Approval");
        }

    }
}
