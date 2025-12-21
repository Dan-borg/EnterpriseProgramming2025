using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;
using Newtonsoft.Json.Linq;

namespace EnterpriseProgramming2025.Presentation.Factory
{
    public class ImportItemFactory
    {
        public List<ItemValidating> Create(string json)
        {
            var result = new List<ItemValidating>();
            var restaurantMap = new Dictionary<string, Restaurant>();

            if (string.IsNullOrWhiteSpace(json))
                return result; // ✅ prevents null parse

            var arr = JArray.Parse(json);

            // 1️⃣ Create restaurants
            foreach (var token in arr.Where(x => x["type"]?.ToString() == "restaurant"))
            {
                var obj = (JObject)token;

                var r = new Restaurant
                {
                    Name = obj["name"]?.ToString() ?? "Unnamed",
                    OwnerEmailAddress = obj["ownerEmailAddress"]?.ToString() ?? "",
                    Description = obj["description"]?.ToString(),
                    Status = "Pending"
                };

                var id = obj["id"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(id))
                    restaurantMap[id] = r;

                result.Add(r);
            }

            // 2️⃣ Create menu items and link them
            foreach (var token in arr.Where(x => x["type"]?.ToString() == "menuItem"))
            {
                var obj = (JObject)token;

                var menuItem = new MenuItem
                {
                    Title = obj["title"]?.ToString() ?? "Unnamed item",
                    Price = double.TryParse(obj["price"]?.ToString(), out var p) ? p : 0,
                    Status = "Pending"
                };

                // handles " restaurantId " with spaces
                var restaurantKey = obj.Properties()
                    .FirstOrDefault(p => p.Name.Trim() == "restaurantId")
                    ?.Value
                    ?.ToString()
                    ?.Trim();

                if (restaurantKey != null &&
                    restaurantMap.TryGetValue(restaurantKey, out var restaurant))
                {
                    menuItem.Restaurant = restaurant;
                    menuItem.RestaurantId = restaurant.Id;
                }

                result.Add(menuItem);
            }

            return result;
        }
    }
}
