using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using salon_manager.Models;

namespace salon_manager.Filters;

public class ForcePasswordChangeFilter : IAsyncActionFilter
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ForcePasswordChangeFilter(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor?.ControllerName != "Account")
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if (user?.MustChangePassword == true)
                {
                    context.Result = new RedirectToActionResult("ChangePassword", "Account", null);
                    return;
                }
            }
        }

        await next();
    }
}
