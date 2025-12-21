using Domain.Interfaces;
using Domain.Models;
using EnterpriseProgramming2025.Data;
using Microsoft.EntityFrameworkCore;


public class ItemsDbRepository : ItemsRepository
{
    private readonly ApplicationDbContext context;

    public ItemsDbRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IQueryable<Restaurant> GetRestaurants(string? status = null)
    {
        var q = context.Restaurants.AsQueryable();
        if (status != null)
            q = q.Where(r => r.Status == status);
        return q;
    }

    public IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string? status = null)
    {
        var q = context.MenuItems
                       .Include(m => m.Restaurant)
                       .AsQueryable();

        if (status != null)
            q = q.Where(m => m.Status == status);

        if (restaurantId.HasValue)
            q = q.Where(m => m.RestaurantId == restaurantId.Value);

        return q;
    }

    public void SaveRestaurants(IEnumerable<Restaurant> restaurants)
    {
        foreach (var r in restaurants)
        {
            // prevent duplicates
            r.Status = "Pending";
            context.Restaurants.Add(r);
        }

        context.SaveChanges();
    }

    public void SaveMenuItems(IEnumerable<MenuItem> items)
    {
        foreach (var m in items)
        {
            if (!context.MenuItems.Any(db => db.Id == m.Id))
            {
                m.Status ??= "Pending";
                context.MenuItems.Add(m);
            }
        }

        context.SaveChanges();
    }

    public void ApproveRestaurants(IEnumerable<int> ids)
    {
        var list = context.Restaurants.Where(r => ids.Contains(r.Id)).ToList();
        foreach (var r in list)
            r.Status = "Approved";

        context.SaveChanges();
    }

    public void ApproveMenuItems(IEnumerable<Guid> ids)
    {
        var list = context.MenuItems.Where(m => ids.Contains(m.Id)).ToList();
        foreach (var m in list)
            m.Status = "Approved";

        context.SaveChanges();
    }

    public void Clear() { }
}
