using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Annotations.Core.Models;

namespace Annotations.Blazor.Client.Services;


public interface IAnnotationDataAccessor
{
    Task<GetAnnotationsResult> GetAnnotationsForImageAsync(string imageId);

    Task<HttpStatusCode> PostAnnotationsForImageAsync(string imageId, VesselAnnotationModel vessel);
}


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
