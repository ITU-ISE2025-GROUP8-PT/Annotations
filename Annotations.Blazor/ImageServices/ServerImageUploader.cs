using MatBlazor;

namespace Annotations.Blazor.ImageServices
{
    public class ServerImageUploader : IImageUploader
    {
        public Task<long> CreateImageSeriesAsync(string name, string category)
        {
            throw new NotImplementedException();
        }

        public Task<string> ImageSeriesAppendAsync(long imageSeriesId, string[] imageIds)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> UploadImagesAsync(IMatFileUploadEntry[] images)
        {
            throw new NotImplementedException();
        }
    }
}
