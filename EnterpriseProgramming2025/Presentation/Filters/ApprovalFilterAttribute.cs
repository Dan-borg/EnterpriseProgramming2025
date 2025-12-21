using System;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EnterpriseProgramming2025.Presentation.Filters
{
    public class ApprovalFilterAttribute : ActionFilterAttribute
    {
        private readonly ItemsRepository dbRepo;

        public ApprovalFilterAttribute([FromKeyedServices("db")] ItemsRepository dbRepo)
        {
            this.dbRepo = dbRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userEmail = context.HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return;

            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is int[] restIds && restIds.Any())
                {
                    var pending = dbRepo.GetRestaurants("Pending")
                                        .Where(r => restIds.Contains(r.Id))
                                        .ToList();
                    if (!pending.All(r => r.GetValidators().Contains(userEmail)))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
                else if (arg is Guid[] menuIds && menuIds.Any())
                {
                    var pending = dbRepo.GetMenuItems(null, "Pending")
                                        .Where(m => menuIds.Contains(m.Id))
                                        .ToList();
                    if (!pending.All(m => m.GetValidators().Contains(userEmail)))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
        }
    }
}
