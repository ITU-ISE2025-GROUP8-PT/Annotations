using MatBlazor;

namespace Annotations.Blazor.ImageServices
{
    public interface IImageUploader
    {
        Task<long> CreateImageSeriesAsync(string name, string category);

        Task<string> ImageSeriesAppendAsync(long imageSeriesId, string[] imageIds);

        Task<string[]> UploadImagesAsync(IMatFileUploadEntry[] images);
    }
}
