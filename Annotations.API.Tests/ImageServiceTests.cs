﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Annotations.API.Services.Datasets;
using Annotations.API.Services.Images;
using Annotations.Core.Entities;
using Annotations.Core.Models;
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
    /// <summary>
    /// Tests the GetImageAsync method of the ImageService class.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetImageAsync_OnValidRequest_ReturnsImage()
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
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: true);


        // Act
        GetImageResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImageAsync(1);
        }

        // Assert
        Assert.Equal(200, result.StatusCode);
    }





    /// <summary>
    /// Tests the GetImageAsync method of the ImageService class when the image is not found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetImageAsync_OnImageNotFound_ReturnsNotFoundResponse()
    {
        // Arrange

        // 1. Database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options; ;

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
            result = await imageService.GetImageAsync(1);
        }


        // Assert
        Assert.Equal(404, result.StatusCode);
    }





    /// <summary>
    /// Tests the DeleteImageAsync method of the ImageService class.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteImageAsync_OnValidRequest_ReturnsNoContent()
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
            context.Add(new Image
            {
                Id = 1,
                Category = "Test",
                CreatedAt = DateTime.UtcNow,
                UploadedBy = new User { UserId = "1", UserName = "Test User" },
            });
            context.SaveChanges();
        }

        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: true);

        // Act
        HttpStatusCode result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.DeleteImageAsync(1);
        }

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result);
    }





    /// <summary>
    /// Tests the DeleteImageAsync method of the ImageService class when the image is not found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteImageAsync_OnImageNotFound_ReturnsNotFound()
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
            context.Add(new Image
            {
                Id = 1,
                Category = "Test",
                CreatedAt = DateTime.UtcNow,
                UploadedBy = new User { UserId = "1", UserName = "Test User" },
            });
            context.SaveChanges();
        }

        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: false);

        // Act
        HttpStatusCode result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.DeleteImageAsync(1);
        }

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result);
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
                .Setup(x => x.DeleteIfExistsAsync(
                    It.IsAny<DeleteSnapshotsOption>(),
                    It.IsAny<BlobRequestConditions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Azure.Response.FromValue(expectedExists, Mock.Of<Azure.Response>())));

            mockBlobClient
                .Setup(x => x.OpenReadAsync(It.IsAny<BlobOpenReadOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Stream>(new MemoryStream([0x01, 0x02, 0x03])));

            mockBlobClient
                .Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Azure.Response.FromValue(new BlobProperties(), Mock.Of<Azure.Response>())));
        }
    }





    /// <summary>
    /// Tests the GetFilteredImageSetAsync method to ensure it returns filtered images.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Filter_ImagesExist_ReturnsFilteredImages()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User
        {
            UserId = "1",
            UserName = "Test User"
        };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 4, Title = "Image4", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }

        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: false);


        // Act
        ICollection<ImageModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImagesByCategoryAsync("Include");
        }


        // Assert
        Assert.Equal(2, result.Count);
    }





    /// <summary>
    /// Tests the GetFilteredImageSetAsync method to ensure it returns an empty list when no images exist.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Filter_ImagesDoNotExist_ReturnsEmptyList()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User
        {
            UserId = "1",
            UserName = "Test User"
        };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }

        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: false);


        // Act
        ICollection<ImageModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImagesByCategoryAsync("Include");
        }


        // Assert
        Assert.Empty(result);
    }





    /// <summary>
    /// Tests that the GetFilteredImageSetAsync method only returns images from the category that are not marked as deleted. 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Filter_ImagesExistWithDeleted_ReturnsFilteredImages()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User
        {
            UserId = "1",
            UserName = "Test User"
        };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.Images.Add(new Image { Id = 4, Title = "Image4", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.SaveChanges();
        }

        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: false);


        // Act
        ICollection<ImageModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImagesByCategoryAsync("Include");
        }


        // Assert
        Assert.Equal(2, result.Count);
    }





    /// <summary>
    /// Tests the GetFilteredImageSetAsync method to ensure it returns an empty list when all images are marked as deleted.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Filter_ImagesDoNotExistWithDeleted_ReturnsEmptyList()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User
        {
            UserId = "1",
            UserName = "Test User"
        };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.Images.Add(new Image { Id = 4, Title = "Image4", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.SaveChanges();
        }

        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: false);


        // Act
        ICollection<ImageModel> result;
        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetImagesByCategoryAsync("Include");
        }


        // Assert
        Assert.Empty(result);
    }





    /// <summary>
    /// Tests the GetMetadataAsync method of the ImageService class to ensure it returns image metadata.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMetadataAsync_OnValidRequest_ReturnsImageMetadata()
    {
        // Arrange

        // 1. Database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User
        {
            UserId = "1",
            UserName = "Test User"
        };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }

        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: true);


        // Act
        ImageModel result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetMetadataAsync(1);
        }


        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }





    /// <summary>
    /// Tests the GetMetadataAsync method of the ImageService class when the image is not found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMetadataAsync_OnImageNotFound_ReturnsNull()
    {
        // Arrange

        // 1. Database context
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User
        {
            UserId = "1",
            UserName = "Test User"
        };

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }

        // 2. Mock Azure Storage
        var mockStore = new MockAzureBlobStorageClientFactory(
            expectedName: "Default",
            expectedContainerName: "images",
            expectedBlobName: "1",
            expectedExists: true);


        // Act
        ImageModel result;

        using (var context = new AnnotationsDbContext(options))
        {
            var imageService = new ImageService(mockStore.MockBlobServiceClientFactory.Object, context);
            result = await imageService.GetMetadataAsync(2);
        }

        // Assert
        Assert.Null(result);
    }
}
