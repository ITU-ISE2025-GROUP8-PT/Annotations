using System.Net.Http.Headers;
using MatBlazor;
using Microsoft.AspNetCore.Authentication;
using Annotations.Core.Results;

namespace Annotations.Blazor.ImageServices
{
    public class ServerImageUploader : IImageUploader
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ServerImageUploader(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public Task<long> CreateImageSeriesAsync(string name, string category)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImageSeriesAppendAsync(long imageSeriesId, string[] imageIds)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> UploadImagesAsync(IMatFileUploadEntry[] images)
        {
            var imageIDs = new List<string>();
            foreach (var image in images)
            {
                imageIDs.Add(await UploadSingleImage(image));
            }
            return imageIDs;
        }

        private async Task<string> UploadSingleImage(IMatFileUploadEntry image)
        {
            var httpContext = _httpContextAccessor.HttpContext ??
                              throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

            var accessToken = await httpContext.GetTokenAsync("access_token") ??
                              throw new InvalidOperationException("No access_token was saved");
            
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Images/Upload");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            var formData = new MultipartFormDataContent();
            var memoryStream = new MemoryStream();
            await image.WriteToStreamAsync(memoryStream);
            formData.Add(new StreamContent(memoryStream), "image", image.Name);
            requestMessage.Content = formData;
            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var imageUploaderResult = await response.Content.ReadFromJsonAsync<ImageUploaderResult>() ??
                throw new IOException("API test unsuccessful. No JSON obtained!");
            return imageUploaderResult.ImageId;
        }
    }
}
