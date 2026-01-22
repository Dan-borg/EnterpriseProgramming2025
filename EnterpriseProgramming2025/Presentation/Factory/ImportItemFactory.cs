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
                return result;

            JArray arr;

            try
            {
                arr = JArray.Parse(json);
            }
            catch
            {
                return result; // invalid JSON → safe exit
            }

            // 1️⃣ Create restaurants
            foreach (var obj in arr.OfType<JObject>()
                        .Where(o => o["type"]?.ToString() == "restaurant"))
            {
                var restaurant = new Restaurant
                {
                    Name = obj["name"]?.ToString() ?? "Unnamed",
                    OwnerEmailAddress = obj["ownerEmailAddress"]?.ToString() ?? string.Empty,
                    Description = obj["description"]?.ToString(),
                    Status = "Pending"
                };

                restaurant.ImportId = obj["id"]?.ToString();

                var importId = ExtractValidId(obj);
                if (importId != null && !restaurantMap.ContainsKey(importId))
                {
                    restaurantMap.Add(importId, restaurant);
                }

                result.Add(restaurant);
            }


            // 2️⃣ Create menu items and link them
            foreach (var obj in arr.OfType<JObject>()
                        .Where(o => o["type"]?.ToString() == "menuItem"))
            {
                var menuItem = new MenuItem
                {
                    Title = obj["title"]?.ToString() ?? "Unnamed item",
                    Price = obj["price"]?.Value<double>() ?? 0,
                    Status = "Pending"
                };

                menuItem.ImportId = obj["id"]?.ToString();

                var restaurantId = obj["restaurantId"]?.ToString()?.Trim();

                if (!string.IsNullOrWhiteSpace(restaurantId) &&
                    restaurantMap.TryGetValue(restaurantId, out var restaurant))
                {
                    menuItem.Restaurant = restaurant;
                }

                result.Add(menuItem);
            }

            return result;
        }

        private static string? ExtractValidId(JObject obj)
        {
            var token = obj["id"];
            if (token == null)

                return null;

            // Numeric 0 → ignore
            if (token.Type == JTokenType.Integer && token.Value<long>() == 0)
                return null;

            var value = token.ToString().Trim();

            if (string.IsNullOrWhiteSpace(value) || value == "0")
                return null;

            return value;
        }
    }
}
