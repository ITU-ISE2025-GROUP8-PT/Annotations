namespace Annotations.API;
using System.IO;

public class ImageRepository
{
    public string ProcessFile(string fileName)
    {
        //validate file
        return "null";
    }

    public string UploadImage(string filepath)
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