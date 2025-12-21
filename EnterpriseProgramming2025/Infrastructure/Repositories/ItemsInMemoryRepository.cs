using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class ItemsInMemoryRepository : ItemsRepository
    {
        private readonly IMemoryCache _cache;
        private const string KEY = "BulkImportItems";

        public ItemsInMemoryRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        private List<ItemValidating> Items =>
            _cache.GetOrCreate(KEY, _ => new List<ItemValidating>());

        public IQueryable<Restaurant> GetRestaurants(string? status = null)
        {
            var q = Items.OfType<Restaurant>().AsQueryable();
            if (status != null)
                q = q.Where(r => r.Status == status);
            return q;
        }

        public IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string? status = null)
        {
            var q = Items.OfType<MenuItem>().AsQueryable();
            if (status != null)
                q = q.Where(m => m.Status == status);
            if (restaurantId.HasValue)
                q = q.Where(m => m.RestaurantId == restaurantId);
            return q;
        }

        public void SaveRestaurants(IEnumerable<Restaurant> restaurants)
        {
            Items.AddRange(restaurants);
        }

        public void SaveMenuItems(IEnumerable<MenuItem> items)
        {
            Items.AddRange(items);
        }

        public void ApproveRestaurants(IEnumerable<int> ids) { }
        public void ApproveMenuItems(IEnumerable<Guid> ids) { }

        public void Clear()
        {
            _cache.Remove(KEY);
        }
    }
}
