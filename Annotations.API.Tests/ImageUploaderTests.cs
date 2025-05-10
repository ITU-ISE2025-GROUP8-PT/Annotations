using System.IO;
using System.Threading.Tasks;
using Annotations.API.Services.Images;
using Annotations.Core.Entities;
using Azure.Storage.Blobs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Moq;
using Xunit;


namespace Annotations.API.Tests;


public class ImageUploaderTests
{
    /// <summary>
    /// Test to see that it is possible to upload an image to the database.
    /// 
    /// Refer to Andrew Lock page 909 for more information on dbContext and in memory database.
    /// </summary>
    [Fact]
    public async Task CanUploadImage()
    {
        // Arrange database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
        }



        // Arrange mock Azure Storage
        var mockBlobServiceClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        var mockBlobContainerClient = new Mock<BlobContainerClient>();
        var mockBlobClient = new Mock<BlobClient>();

        mockBlobServiceClientFactory
            .Setup(x => x.CreateClient(It.Is<string>(s => s.Equals("Default"))))
            .Returns(mockBlobServiceClient.Object);

        mockBlobServiceClient.
            Setup(x => x.GetBlobContainerClient(It.Is<string>(s => s.Equals("images"))))
            .Returns(mockBlobContainerClient.Object);

        mockBlobContainerClient
            .Setup(x => x.GetBlobClient(It.Is<string>(s => s.Equals("0"))))
            .Returns(mockBlobClient.Object);



        // Act
        ImageUploaderResult uploadResult;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageUploader = new ImageUploader(context, mockBlobServiceClientFactory.Object);
            imageUploader.OriginalFilename = "test.jpg";
            imageUploader.ContentType = "image/jpeg";
            imageUploader.InputStream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });
            imageUploader.UploadedBy = new User { UserId = "1", UserName = "Test User" };

            uploadResult = await imageUploader.StoreAsync();
        }



        // Assert
        Assert.Equal(201, uploadResult.StatusCode);
        Assert.Equal(0, uploadResult.ImageId);
        Assert.Equal(string.Empty, uploadResult.Error);
    }
}
