using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Annotations.Core.Models;

namespace Annotations.Blazor.Client.Services;


/// <summary>
/// Defines the contract for the annotation data accessor service.
/// </summary>
public interface IAnnotationDataAccessor
{
    /// <summary>
    /// Retrieves all annotations for a given image. This will return a list of vessel annotations.
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    Task<GetAnnotationsResult> GetAnnotationsForImageAsync(string imageId);

    /// <summary>
    /// Posts a vessel annotation for a given image. This will save the annotation to the backend API.
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="vessel"></param>
    /// <returns></returns>
    Task<HttpStatusCode> PostAnnotationsForImageAsync(string imageId, VesselAnnotationModel vessel);
}



/// <summary>
/// This service is used to access the annotation data.
/// </summary>
/// <param name="httpClient"></param>
public sealed class AnnotationDataAccessor(HttpClient httpClient) : IAnnotationDataAccessor
{
    public async Task<GetAnnotationsResult> GetAnnotationsForImageAsync(string imageId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/images/annotations?imagePath={Uri.EscapeDataString(imageId)}");
        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await response.Content.ReadFromJsonAsync<List<VesselAnnotationModel>>(options);

            return new GetAnnotationsResult
            {
                Annotations = result ?? [],
                IsSuccess = true
            };
        }
        else
        {
            return new GetAnnotationsResult
            {
                Annotations = [],
                IsSuccess = false
            };
        }
    }


    public Task<HttpStatusCode> PostAnnotationsForImageAsync(string imageId, VesselAnnotationModel vessel)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/images/annotations/save");
        request.Content = JsonContent.Create(vessel);
        return httpClient.SendAsync(request).ContinueWith(t => t.Result.StatusCode);
    }
}


public sealed class GetAnnotationsResult
{
    public List<VesselAnnotationModel> Annotations { get; set; } = [];

    public bool IsSuccess { get; set; } = false;
}
