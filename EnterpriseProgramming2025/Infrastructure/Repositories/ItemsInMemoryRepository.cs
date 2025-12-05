using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

public class ItemsInMemoryRepository : IItemsRepository
{
    private readonly IMemoryCache _cache;

    public ItemsInMemoryRepository(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IQueryable<Restaurant> GetRestaurants(string status = "Pending")
    {
        var list = _cache.Get<List<Restaurant>>("pending_restaurants") ?? new List<Restaurant>();
        return list.AsQueryable();
    }

    public IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string status = "Pending")
    {
        var list = _cache.Get<List<MenuItem>>("pending_menuitems") ?? new List<MenuItem>();
        return list.AsQueryable();
    }

    public void SaveRestaurants(IEnumerable<Restaurant> restaurants)
    {
        _cache.Set("pending_restaurants", restaurants.ToList());
    }

    public void SaveMenuItems(IEnumerable<MenuItem> items)
    {
        _cache.Set("pending_menuitems", items.ToList());
    }

    public void ApproveRestaurants(IEnumerable<int> ids) { }
    public void ApproveMenuItems(IEnumerable<Guid> ids) { }
}
