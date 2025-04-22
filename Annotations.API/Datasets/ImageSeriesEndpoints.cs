using Annotations.API.Images;
using Annotations.API.Users;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Annotations.API.Datasets;

public class ImageSeriesEndpoints
{
    /// <summary>
    /// Maps all application endpoints for image series creation, retrieval and manipulation.
    /// </summary>
    /// <param name="groupBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization()
            .DisableAntiforgery(); // Disabled as this is an API, which will not serve any forms.
        groupBuilder.MapPost("/New", NewImageSeriesHandler);
    }


    static async Task<ImageSeriesBuilderResult> NewImageSeriesHandler(
        string name,
        string category,
        ClaimsPrincipal claimsPrincipal,
        HttpContext     httpContext,
        [FromServices] IImageSeriesBuilder builder,
        [FromServices] IUserService userService
        )
    {
        var user = await userService.TryFindUserAsync(claimsPrincipal) ?? await userService.CreateUser(claimsPrincipal);

        builder.Name = name;
        builder.CreatedBy = user;
        builder.Category = category;

        var builderResult = await builder.BuildAsync();

        httpContext.Response.StatusCode = builderResult.StatusCode;
        return builderResult;
    }
}
