using Microsoft.AspNetCore.Identity;
using Annotations.Core.Entities;

namespace Annotations.Blazor.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<AnnotationsUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<AnnotationsUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
