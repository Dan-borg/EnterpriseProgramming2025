using Domain.Interfaces;
using Domain.Models;
using Newtonsoft.Json.Linq;

namespace EnterpriseProgramming2025.Presentation.Factory
{
    public class ImportItemFactory
    {
        public List<IItemValidating> Create(string json)
        {
            var list = new List<IItemValidating>();

            // Parse input JSON as an array
            var arr = JArray.Parse(json);

            foreach (var obj in arr)
            {
                var type = (string)obj["type"];

                if (type == "restaurant")
                {
                    var restaurant = new Restaurant
                    {
                        Name = (string)obj["name"],
                        OwnerEmailAddress = (string)obj["email"],
                        Description = (string)obj["description"] ?? "",
                        Status = "Pending"
                    };

                    list.Add(restaurant);
                }
                else if (type == "menuItem")
                {
                    var menu = new MenuItem
                    {
                        Title = (string)obj["name"],
                        Price = (decimal?)obj["price"] ?? 0,
                        RestaurantId = (int)obj["restaurantId"],
                        Status = "Pending"
                    };

                    list.Add(menu);
                }
            }

            return list;
        }
    }
}
