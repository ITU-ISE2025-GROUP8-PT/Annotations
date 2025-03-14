using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Annotations.API.Groups;

public class AccountsGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapPost("/register", async (UserManager<AnnotationsUser> userManager, RegisterModel model) =>
        {
            var newUser = new AnnotationsUser { UserName = model.Email, Email = model.Email };

            var result = await userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return new RegisterResult { Successful = false, Errors = errors };

            }
            return new RegisterResult { Successful = true };
        });
    }
}