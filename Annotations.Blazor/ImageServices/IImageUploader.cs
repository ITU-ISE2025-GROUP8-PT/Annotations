using MatBlazor;

namespace Annotations.Blazor.ImageServices
{
    public interface IImageUploader
    {
        Task<long> CreateImageSeriesAsync(string name, string category);

        Task<string> ImageSeriesAppendAsync(long imageSeriesId, string[] imageIds);

        Task<IList<string>> UploadImagesAsync(IMatFileUploadEntry[] images);
    }
}
