using Domain.Models;

namespace Domain.Interfaces
{
    public interface IItemsRepository
    {
        IQueryable<Restaurant> GetRestaurants(string status = "Approved");
        IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string status = "Approved");

        void SaveRestaurants(IEnumerable<Restaurant> restaurants);
        void SaveMenuItems(IEnumerable<MenuItem> items);

        void ApproveRestaurants(IEnumerable<int> ids);
        void ApproveMenuItems(IEnumerable<Guid> ids);
    }
}
