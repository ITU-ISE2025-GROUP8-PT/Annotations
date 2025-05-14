using System.Security.Claims;
using Annotations.API.Services.Datasets;
using Annotations.API.Services.Users;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;


namespace Annotations.API.Endpoints;


/// <summary>
/// Endpoints for datasets. Collections of images.
/// </summary>
public class DatasetEndpoints
{
    /// <summary>
    /// Where all the endpoints are initialized to their respective handler. 
    /// </summary>
    /// <param name="pathBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        // Anti-forgery is disabled. This was decided because the backend will not serve any forms.
        // Anti-forgery measures are covered in the front-end, and by the JWT token protection. 
        pathBuilder.RequireAuthorization().DisableAntiforgery();

        pathBuilder.MapGet("/filter/{category}", FilterImagesHandler);

        pathBuilder.MapGet("/overview", RetrieveAllDatasetHandler);

        pathBuilder.MapGet("/get/{datasetId}", RetrieveSingleDatasetHandler);

        pathBuilder.MapPost("/create", CreateDatasetHandler);

        pathBuilder.MapDelete("/delete/{datasetId}", DeleteDatasetHandler);

        pathBuilder.MapPut("/set-images/{datasetId}", SetImagesHandler);
    }






    /// <summary>
    /// Finds and returns all images within a certain category.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> An array of images as JSON string with the wanted category. </returns>
    private static async Task<ICollection<ImageModel>> FilterImagesHandler(
        [FromRoute] string category,
        [FromServices] IDatasetService datasetService)
    {
        return await datasetService.GetFilteredImageSetAsync(category);
    }





    /// <summary>
    /// Retrieves all existing datasets. 
    /// </summary>
    /// <param name="context"> Annotations database context containing the datasets. </param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> A collection of all existing DatasetModels. </returns>
    private static async Task<ICollection<DatasetModel>> RetrieveAllDatasetHandler(
        [FromServices] IDatasetService datasetService)
    {
        return await datasetService.GetDatasetOverviewAsync();
    }





    /// <summary>
    /// Retrieves all the images inside a specific dataset. 
    /// </summary>
    /// <param name="datasetId"> ID of the desired dataset. </param>
    /// <param name="context"> Annotations database context containing the datasets. </param>
    /// <param name="imageService"> An image service instance. </param>
    /// <returns> A string array of the needed images as a JSON string. </returns>
    private static async Task<IResult> RetrieveSingleDatasetHandler(
        [FromRoute] int datasetId,
        [FromServices] IDatasetService datasetService)
    {
        var datasetModel = await datasetService.GetDatasetByIdAsync(datasetId);
        return datasetModel != null
            ? Results.Ok(datasetModel)
            : Results.NotFound();
    }





    /// <summary>
    /// Creates a new dataset.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="category"></param>
    /// <param name="claimsPrincipal"></param>
    /// <param name="httpContext"></param>
    /// <param name="datasetBuilder"></param>
    /// <param name="userService"></param>
    /// <returns></returns>
    private static async Task<ModifyDatasetResult> CreateDatasetHandler(
        string          title,
        string          category,
        ClaimsPrincipal claimsPrincipal,
        HttpContext     httpContext,
        [FromServices] IDatasetBuilder datasetBuilder,
        [FromServices] IUserService userService)
    {
        var user = await userService.TryFindUserAsync(claimsPrincipal) ?? await userService.CreateUserAsync(claimsPrincipal);

        datasetBuilder.Title = title;
        datasetBuilder.CreatedBy = user;
        datasetBuilder.Category = category;

        var result = await datasetBuilder.BuildAsync();

        httpContext.Response.StatusCode = result.StatusCode;
        return result;
    }





    private static async Task<IResult> DeleteDatasetHandler(
        [FromRoute] int datasetId,
        [FromServices] IDatasetService datasetService)
    {
        return Results.StatusCode((int)await datasetService.DeleteDatasetAsync(datasetId));
    }





    /// <summary>
    /// Sets the images in a dataset.
    /// </summary>
    /// <param name="datasetId"></param>
    /// <param name="imageIds"></param>
    /// <param name="httpContext"></param>
    /// <param name="datasetService"></param>
    /// <returns></returns>
    private static async Task<ModifyDatasetResult> SetImagesHandler(
        [FromRoute] int datasetId,
        int[] imageIds,
        HttpContext httpContext,
        [FromServices] IDatasetService datasetService)
    {
        var result = await datasetService.SetImagesAsync(datasetId, imageIds);

        httpContext.Response.StatusCode = result.StatusCode;
        return result;
    }
}
