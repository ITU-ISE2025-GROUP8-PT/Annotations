using Annotations.API.Services.Datasets;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Annotations.API.Endpoints;


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
        return await datasetService.Filter(category);
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
        return await datasetService.GetDatasetOverview();
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
        var datasetModel = await datasetService.GetSingleDataset(datasetId);
        return datasetModel != null
            ? Results.Ok(datasetModel)
            : Results.NotFound();
    }
}
