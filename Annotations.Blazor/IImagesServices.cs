namespace Annotations.Blazor;

public interface IImagesServices
{
    Task<HttpResponseMessage> createGetResponse(string requestURI);
}