using System;
using System.IO;
using System.Linq;
using System.Threading;
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
    /// Initially, the database is created in memory and populated with 3 Image entries.
    /// This means that the next image will be assigned ID 4.
    /// 
    /// Refer to Andrew Lock page 909 for more information on dbContext and in memory database.
    /// </summary>
    [Fact]
    public async Task CanUploadImage()
    {
        // Arrange
        
        // 1. Database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User { UserId = "1", UserName = "Test User" };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            for (int i = 1; i < 4; i++) {
                context.Add(new Image 
                {
                    Id = i,
                    CreatedAt = DateTime.UtcNow,
                    UploadedBy = testUser,
                });
            }
            context.SaveChanges();
        }

        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "4");



        // Act
        ImageUploaderResult uploadResult;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageUploader = new ImageUploader(context, mockStore.MockBlobServiceClientFactory.Object);
            imageUploader.OriginalFilename = "test.jpg";
            imageUploader.ContentType = "image/jpeg";
            imageUploader.InputStream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });
            imageUploader.UploadedBy = context.Users.Where(u => u.UserId == "1").Single();

            uploadResult = await imageUploader.StoreAsync();
        }



        // Assert
        Assert.Equal(201, uploadResult.StatusCode);
        Assert.Equal(4, uploadResult.ImageId);
        Assert.Equal(string.Empty, uploadResult.Error);
    }
}



/// <summary>
/// Class to arrange the mock Azure Storage client factory.
/// </summary>
sealed class MockAzureBlobStorageClientFactory
{
    public Mock<IAzureClientFactory<BlobServiceClient>> MockBlobServiceClientFactory { get; } = new();

    public MockAzureBlobStorageClientFactory(
        string expectedName, 
        string expectedContainerName,
        string expectedBlobName)
    {
        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        var mockBlobContainerClient = new Mock<BlobContainerClient>();
        var mockBlobClient = new Mock<BlobClient>();

        MockBlobServiceClientFactory
            .Setup(x => x.CreateClient(It.Is<string>(s => s.Equals(expectedName))))
            .Returns(mockBlobServiceClient.Object);

        mockBlobServiceClient.
            Setup(x => x.GetBlobContainerClient(It.Is<string>(s => s.Equals(expectedContainerName))))
            .Returns(mockBlobContainerClient.Object);

        mockBlobContainerClient
            .Setup(x => x.GetBlobClient(It.Is<string>(s => s.Equals(expectedBlobName))))
            .Returns(mockBlobClient.Object);

        mockBlobClient
            .Setup(x => x.Exists(default))
            .Returns(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()));
    }
}
