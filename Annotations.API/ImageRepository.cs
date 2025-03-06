namespace Annotations.API;
using System.IO;

public class ImageRepository
{
    public async Task<> ProcessFile(IFormFile file)
    {
        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest("File size is too big");
        }
        else if (if file == null || file.Length == 0)
        {
             return BadRequest("File doen't exist");
        }
        else if (file.ContentType != "image/jpeg" || file.ContentType != "image/png")
        {
            return BadRequest("File is not a jpeg or png");
        }
        else
        {
            return Ok("File validated")
        }
    }

    public string UploadImage(IFormFile filepath)
    {
        ProcessFile(filepath);
        //add file to db
        return "null";
    }

    public string DownloadImage(string filepath)
    {
        //retrieve image form db
        return "null";
    }

    public string DeleteImage(string filepath)
    {
        return "null";
    }
    
    
}