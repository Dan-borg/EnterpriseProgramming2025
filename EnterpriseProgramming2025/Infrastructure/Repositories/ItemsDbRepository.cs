using Domain.Interfaces;
using Domain.Models;
using EnterpriseProgramming2025.Data;
using Microsoft.EntityFrameworkCore;

public class ItemsDbRepository : IItemsRepository
{
    private readonly ApplicationDbContext _context;

    public ItemsDbRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Restaurant> GetRestaurants(string status = "Approved")
    {
        return _context.Restaurants.Where(r => r.Status == status);
    }

    public IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string status = "Approved")
    {
        var query = _context.MenuItems
            .Include(m => m.Restaurant)
            .Where(m => m.Status == status);

        if (restaurantId != null)
            query = query.Where(m => m.RestaurantId == restaurantId);

        return query;
    }

    public void SaveRestaurants(IEnumerable<Restaurant> restaurants)
    {
        _context.Restaurants.AddRange(restaurants);
        _context.SaveChanges();
    }

    public void SaveMenuItems(IEnumerable<MenuItem> items)
    {
        _context.MenuItems.AddRange(items);
        _context.SaveChanges();
    }

    public void ApproveRestaurants(IEnumerable<int> ids)
    {
        var items = _context.Restaurants.Where(r => ids.Contains(r.Id)).ToList();
        foreach (var item in items) item.Status = "Approved";
        _context.SaveChanges();
    }

    public void ApproveMenuItems(IEnumerable<Guid> ids)
    {
        var items = _context.MenuItems.Where(m => ids.Contains(m.Id)).ToList();
        foreach (var item in items) item.Status = "Approved";
        _context.SaveChanges();
    }
}
