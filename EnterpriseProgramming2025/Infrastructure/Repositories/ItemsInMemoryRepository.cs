using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories;

public class ItemsInMemoryRepository : IItemsRepository
{
    private List<Restaurant> _restaurants = new();
    private List<MenuItem> _menuItems = new();

    public IQueryable<Restaurant> GetRestaurants(string status = "Approved")
    {
        return _restaurants
            .Where(r => r.Status == status)
            .AsQueryable();
    }

    public IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string status = "Approved")
    {
        var items = _menuItems.Where(m => m.Status == status);

        if (restaurantId != null)
            items = items.Where(m => m.RestaurantId == restaurantId);

        return items.AsQueryable();
    }

    public void SaveRestaurants(IEnumerable<Restaurant> restaurants)
    {
        _restaurants = restaurants.ToList();
    }

    public void SaveMenuItems(IEnumerable<MenuItem> items)
    {
        _menuItems = items.ToList();
    }

    public void ApproveRestaurants(IEnumerable<int> ids)
    {
        foreach (var r in _restaurants.Where(r => ids.Contains(r.Id)))
            r.Status = "Approved";
    }

    public void ApproveMenuItems(IEnumerable<Guid> ids)
    {
        foreach (var m in _menuItems.Where(m => ids.Contains(m.Id)))
            m.Status = "Approved";
    }
}
