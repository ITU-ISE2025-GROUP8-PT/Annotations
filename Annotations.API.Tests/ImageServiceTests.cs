using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Annotations.API.Services.Images;
using Annotations.Core.Entities;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Moq;
using Xunit;


namespace Annotations.API.Tests;

public class ImageServiceTests
{
    [Fact]
    public async Task OnValidRequest_CanGetImage()
    {
        // Arrange

        // 1. Database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
        }


        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName:          "Default",
            expectedContainerName: "images",
            expectedBlobName:      "1",
            expectedExists:        true);


        // Act
        GetImageResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImageAsync("1");
        }

        // Assert
        Assert.Equal(200, result.StatusCode);
    }





    [Fact]
    public async Task OnImageNotFound_ReturnsNotFoundResponse()
    {
        // Arrange

        // 1. Database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;;

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
        }


        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: false);


        // Act
        GetImageResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImageAsync("1");
        }


        // Assert
        Assert.Equal(404, result.StatusCode);
    }





    /// <summary>
    /// Class to arrange the mock Azure Storage client factory.
    /// </summary>
    private sealed class MockAzureBlobStorageClientFactory
    {
        public Mock<IAzureClientFactory<BlobServiceClient>> MockBlobServiceClientFactory { get; } = new();

        public MockAzureBlobStorageClientFactory(
            string expectedName,
            string expectedContainerName,
            string expectedBlobName,
            bool expectedExists)
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
                .Setup(x => x.Exists(It.IsAny<CancellationToken>()))
                .Returns(Azure.Response.FromValue(expectedExists, Mock.Of<Azure.Response>()));

            mockBlobClient
                .Setup(x => x.OpenReadAsync(It.IsAny<BlobOpenReadOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Stream>(new MemoryStream([0x01, 0x02, 0x03])));

            mockBlobClient
                .Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Azure.Response.FromValue(new BlobProperties(), Mock.Of<Azure.Response>())));
        }
    }
}
