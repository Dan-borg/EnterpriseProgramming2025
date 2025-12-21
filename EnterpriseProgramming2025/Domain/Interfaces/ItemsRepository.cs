using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Interfaces
{
    public interface ItemsRepository
    {
        IQueryable<Restaurant> GetRestaurants(string? status = null);
        IQueryable<MenuItem> GetMenuItems(int? restaurantId = null, string? status = null);

        void SaveRestaurants(IEnumerable<Restaurant> restaurants);
        void SaveMenuItems(IEnumerable<MenuItem> items);

        void ApproveRestaurants(IEnumerable<int> ids);
        void ApproveMenuItems(IEnumerable<Guid> ids);

        void Clear();
    }
}
