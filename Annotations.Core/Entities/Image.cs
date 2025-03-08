namespace Annotations.Core.Entities;

public class Image
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public byte[] ImageData { get; set; }
}