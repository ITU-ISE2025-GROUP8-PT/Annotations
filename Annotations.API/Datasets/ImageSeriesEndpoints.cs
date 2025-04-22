using Annotations.API.Images;
using Annotations.API.Users;
using Annotations.Core.Entities;
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
        groupBuilder.MapGet("/Get/{imageSeriesId}", GetImageSeriesHandler);
        groupBuilder.MapDelete("/Delete/{imageSeriesId}", DeleteImageSeriesHandler);
        groupBuilder.MapPut("/Append/{imageSeriesId}", AppendImagesHandler);
    }


    /// <summary>
    /// Handler for post request to create an image series.
    /// </summary>
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


    /// <summary>
    /// Handler for get request to obtain an image series.
    /// </summary>
    static async Task<ImageSeries?> GetImageSeriesHandler(
        [FromRoute] long imageSeriesId,
        HttpContext httpContext,
        [FromServices] IImageSeriesService imageSeriesService
        )
    {
        var getImageSeriesResult = await imageSeriesService.GetImageSeriesAsync(imageSeriesId);

        httpContext.Response.StatusCode = getImageSeriesResult.StatusCode;
        return getImageSeriesResult.ImageSeries;
    }


    static async Task<IResult> DeleteImageSeriesHandler(
        [FromRoute] long imageSeriesId,
        [FromServices] IImageSeriesService imageSeriesService
        )
    {
        var httpResult = await imageSeriesService.DeleteImageSeriesAsync(imageSeriesId);

        return Results.StatusCode((int) httpResult);
    }


    static async Task<AppendImagesResult> AppendImagesHandler(
        string[] imageIds,
        HttpContext httpContext,
        [FromServices] IImageSeriesService imageSeriesService
        )
    {
        var appendImagesResult = await imageSeriesService.AppendImagesAsync(imageIds);

        httpContext.Response.StatusCode = appendImagesResult.StatusCode;
        return appendImagesResult;
    }
}
