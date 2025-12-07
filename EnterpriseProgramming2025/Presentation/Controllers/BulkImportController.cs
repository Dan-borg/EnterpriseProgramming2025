using System.IO.Compression;
using Domain.Interfaces;
using Domain.Models;
using EnterpriseProgramming2025.Presentation.Factory;
using EnterpriseProgramming2025.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseProgramming2025.Presentation.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly ImportItemFactory _factory;
        private readonly IItemsRepository _memoryRepo;

        public BulkImportController(
            ImportItemFactory factory,
            [FromKeyedServices("memory")] IItemsRepository memoryRepo)
        {
            _factory = factory;
            _memoryRepo = memoryRepo;
        }

        // First page: user pastes JSON
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: convert JSON → objects and show preview
        [HttpPost]
        [ValidateItem]
        public IActionResult Preview(string jsonInput)
        {
            if (string.IsNullOrWhiteSpace(jsonInput))
            {
                ViewBag.Error = "JSON input is required.";
                return View("Index");
            }

            // Parse JSON into models
            var items = _factory.Create(jsonInput);

            // Separate by type
            var restaurants = items.OfType<Restaurant>().ToList();
            var menuItems = items.OfType<MenuItem>().ToList();

            // Save to memory repo (preview only, not DB yet)
            _memoryRepo.SaveRestaurants(restaurants);
            _memoryRepo.SaveMenuItems(menuItems);

            return View(items); // Show preview page
        }

        // ZIP generation placeholder
        [HttpPost]
        public IActionResult GenerateZip()
        {
            // Load pending items from the memory repo
            var restaurants = _memoryRepo.GetRestaurants().ToList();
            var menuItems = _memoryRepo.GetMenuItems().ToList();

            // Create temporary folder
            var tempFolder = Path.Combine(Path.GetTempPath(), "importZip_" + Guid.NewGuid());
            Directory.CreateDirectory(tempFolder);

            // Create subfolders
            var restaurantsFolder = Path.Combine(tempFolder, "restaurants");
            var menuItemsFolder = Path.Combine(tempFolder, "menuitems");
            Directory.CreateDirectory(restaurantsFolder);
            Directory.CreateDirectory(menuItemsFolder);

            // Path to a default image (we will provide one)
            var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "default.jpg");

            // If you don't have a default.jpg yet, create a blank file
            if (!System.IO.File.Exists(defaultImagePath))
            {
                System.IO.File.WriteAllText(defaultImagePath, "placeholder");
            }

            // Build folder structure: restaurants
            foreach (var r in restaurants)
            {
                var folder = Path.Combine(restaurantsFolder, r.Id.ToString());
                Directory.CreateDirectory(folder);

                var dest = Path.Combine(folder, "default.jpg");
                System.IO.File.Copy(defaultImagePath, dest, true);
            }

            // Build folder structure: menu items
            foreach (var m in menuItems)
            {
                var folder = Path.Combine(menuItemsFolder, m.Id.ToString());
                Directory.CreateDirectory(folder);

                var dest = Path.Combine(folder, "default.jpg");
                System.IO.File.Copy(defaultImagePath, dest, true);
            }

            // Create ZIP file
            var zipPath = tempFolder + ".zip";
            ZipFile.CreateFromDirectory(tempFolder, zipPath);

            var zipBytes = System.IO.File.ReadAllBytes(zipPath);

            // Return the file to the user
            return File(zipBytes, "application/zip", "images.zip");
        }


        // Commit to database placeholder
        [HttpPost]
        [ValidateItem]
        public IActionResult Commit(IFormFile zipFile,
    [FromKeyedServices("db")] IItemsRepository dbRepo)
        {
            if (zipFile == null || zipFile.Length == 0)
                return Content("ZIP file is required.");

            // 1) Create temp folder
            var tempFolder = Path.Combine(Path.GetTempPath(), "uploadedZip_" + Guid.NewGuid());
            Directory.CreateDirectory(tempFolder);

            // 2) Save uploaded ZIP to temp
            var zipPath = Path.Combine(tempFolder, "uploaded.zip");
            using (var stream = new FileStream(zipPath, FileMode.Create))
            {
                zipFile.CopyTo(stream);
            }

            // 3) Extract ZIP
            ZipFile.ExtractToDirectory(zipPath, tempFolder);

            // 4) Get restaurants and menu items from memory repo
            var restaurants = _memoryRepo.GetRestaurants().ToList();
            var menuItems = _memoryRepo.GetMenuItems().ToList();

            // 5) Create target images folder
            var imagesRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(imagesRoot);

            // Copy restaurant images
            foreach (var r in restaurants)
            {
                var srcFolder = Path.Combine(tempFolder, "restaurants", r.Id.ToString());

                if (Directory.Exists(srcFolder))
                {
                    var destFolder = Path.Combine(imagesRoot, "restaurants", r.Id.ToString());
                    Directory.CreateDirectory(destFolder);

                    foreach (var file in Directory.GetFiles(srcFolder))
                    {
                        var fileName = Path.GetFileName(file);
                        var target = Path.Combine(destFolder, fileName);

                        System.IO.File.Copy(file, target, true);
                        r.ImagePath = "/images/restaurants/" + r.Id + "/" + fileName;
                    }
                }
            }

            // Copy menu item images
            foreach (var m in menuItems)
            {
                var srcFolder = Path.Combine(tempFolder, "menuitems", m.Id.ToString());

                if (Directory.Exists(srcFolder))
                {
                    var destFolder = Path.Combine(imagesRoot, "menuitems", m.Id.ToString());
                    Directory.CreateDirectory(destFolder);

                    foreach (var file in Directory.GetFiles(srcFolder))
                    {
                        var fileName = Path.GetFileName(file);
                        var target = Path.Combine(destFolder, fileName);

                        System.IO.File.Copy(file, target, true);
                        m.ImagePath = "/images/menuitems/" + m.Id + "/" + fileName;
                    }
                }
            }

            // 6) Save to database repo
            dbRepo.SaveRestaurants(restaurants);
            dbRepo.SaveMenuItems(menuItems);

            // 7) memory repo
            _memoryRepo.SaveRestaurants(new List<Restaurant>());
            _memoryRepo.SaveMenuItems(new List<MenuItem>());


            // 8) Redirect to catalog
            return RedirectToAction("Catalog", "Items");
        }

    }
}
