using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Annotations.API.Services.Datasets;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Annotations.API.Tests;


public class DatasetServiceTests
{
    [Fact]
    public async Task GetDatasetsAsync_ReturnsAllDatasets()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.Datasets.Add(new Dataset { Id = 2, Title = "Dataset2", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.SaveChanges();
        }


        // Act
        ICollection<DatasetModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetDatasetOverviewAsync();
        }


        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Title == "Dataset1");
        Assert.Contains(result, d => d.Title == "Dataset2");
    }





    [Fact]
    public async Task GetDatasetByIdAsync_DatasetExists_ReturnsDataset()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.SaveChanges();
        }


        // Act
        DatasetModel result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetDatasetByIdAsync(1);
        }


        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dataset1", result.Title);
    }





    [Fact]
    public async Task GetDatasetByIdAsync_DatasetDoesNotExist_ReturnsNull()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.SaveChanges();
        }


        // Act
        DatasetModel result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetDatasetByIdAsync(2);
        }


        // Assert
        Assert.Null(result);
    }





    [Fact]
    public async Task DeleteDatasetAsync_DatasetExists_DeletesDataset()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.SaveChanges();
        }


        // Act
        HttpStatusCode result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.DeleteDatasetAsync(1);
        }


        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result);
    }





    [Fact]
    public async Task DeleteDatasetAsync_DatasetDoesNotExist_ReturnsNotFound()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.SaveChanges();
        }


        // Act
        HttpStatusCode result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.DeleteDatasetAsync(2);
        }


        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result);
    }





    [Fact]
    public async Task DeleteDatasetAsync_DatasetIsDeleted_ReturnsNotFound()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser, IsDeleted = true });
            context.SaveChanges();
        }


        // Act
        HttpStatusCode result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.DeleteDatasetAsync(1);
        }


        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result);
    }





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


        // Act
        ICollection<ImageModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetFilteredImageSetAsync("Include");
        }


        // Assert
        Assert.Equal(2, result.Count);
    }





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


        // Act
        ICollection<ImageModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetFilteredImageSetAsync("Include");
        }


        // Assert
        Assert.Empty(result);
    }





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


        // Act
        ICollection<ImageModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetFilteredImageSetAsync("Include");
        }


        // Assert
        Assert.Equal(2, result.Count);
    }





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
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.Images.Add(new Image { Id = 4, Title = "Image4", Category = "Exclude", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.SaveChanges();
        }


        // Act
        ICollection<ImageModel> result;
        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.GetFilteredImageSetAsync("Include");
        }


        // Assert
        Assert.Empty(result);
    }





    [Fact]
    public async Task SetImagesAsync_ValidRequest_ImagesAreSet()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.SetImagesAsync(1, [1, 2, 3]);
        }


        // Assert
        Assert.Equal(201, result.StatusCode);
        Assert.Equal(3, result.Dataset.ImageIds.Count);
    }





    [Fact]
    public async Task SetImagesAsync_DatasetDoesNotExist_ReturnsNotFound()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.SetImagesAsync(2, [1, 2, 3]);
        }


        // Assert
        Assert.Equal(404, result.StatusCode);
    }





    [Fact]
    public async Task SetImagesAsync_ImagesDoNotExist_ReturnsNotFound()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.SetImagesAsync(1, [1, 2, 4]);
        }


        // Assert
        Assert.Equal(404, result.StatusCode);
    }





    [Fact]
    public async Task SetImagesAsync_ImagesAreDeleted_ReturnsNotFound()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser, IsDeleted = true });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.SetImagesAsync(1, [1, 2, 3]);
        }


        // Assert
        Assert.Equal(404, result.StatusCode);
    }





    [Fact]
    public async Task SetImagesAsync_DuplicatesInSequence_ReturnsBadRequest()
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
            context.Datasets.Add(new Dataset { Id = 1, Title = "Dataset1", Category = "Test", CreatedAt = DateTime.UtcNow, CreatedBy = testUser });
            context.Images.Add(new Image { Id = 1, Title = "Image1", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 2, Title = "Image2", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.Images.Add(new Image { Id = 3, Title = "Image3", Category = "Include", CreatedAt = DateTime.UtcNow, UploadedBy = testUser });
            context.SaveChanges();
        }


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetService = new DatasetService(context);
            result = await datasetService.SetImagesAsync(1, [1, 2, 2, 3]);
        }


        // Assert
        Assert.Equal(400, result.StatusCode);
    }
}
