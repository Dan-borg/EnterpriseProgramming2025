using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EnterpriseProgramming2025.Presentation.Filters
{
    public class ValidateItemAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is Restaurant r)
                {
                    if (string.IsNullOrWhiteSpace(r.Name) ||
                        string.IsNullOrWhiteSpace(r.OwnerEmailAddress))
                    {
                        context.Result = new BadRequestObjectResult("Restaurant must have Name and Owner Email.");
                        return;
                    }
                }

                if (arg is MenuItem m)
                {
                    if (string.IsNullOrWhiteSpace(m.Title) || m.Price <= 0)
                    {
                        context.Result = new BadRequestObjectResult("MenuItem must have Title and Price > 0.");
                        return;
                    }
                }
            }
        }
    }
}
