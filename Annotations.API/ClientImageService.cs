namespace Annotations.API.Images;

public class ClientImageService(HttpClient http) : IImageService
{
    public async Task<Image[]> GetImagesAsync(/*bool watchedMovies*/)
    {
        UploadedImages = await http.GetFromJsonAsync<Image[]>("images/upload") ?? [];
        Console.WriteLine(UploadedImages);
    }
        
}

