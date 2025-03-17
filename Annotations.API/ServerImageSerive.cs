namespace Annotations.API.Images;
public class ServerImageService(AnnotationsDbContext _context) : IImageService
{
    private readonly AnnotationsDbContext _context;
    public ServerImageService(AnnotationsDbContext context)
	{
		_context = context;
	}
    public async Task<Image[]> GetImagesAsync(/*bool watchedMovies*/)
    {
        ImageTitles = await _context.Images.Include(I => I.Title).ToArrayAsync();
        Console.WriteLine(ImageTitles);
    }
}