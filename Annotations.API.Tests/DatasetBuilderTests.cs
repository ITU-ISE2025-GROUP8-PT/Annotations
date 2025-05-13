using System;
using System.Linq;
using System.Threading.Tasks;
using Annotations.API.Services.Datasets;
using Annotations.Core.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Annotations.API.Tests;

public class DatasetBuilderTests
{
    [Fact]
    public async Task OnValidRequest_CanBuildDataset()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        var testUser = new User { UserId = "1", UserName = "Test User" };

        // Adds a test user and a dataset to the in-memory database
        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Add(new Dataset
            {
                Id = 1,
                Title = "Test Dataset",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = testUser,
                Category = "Test",
            });
            context.SaveChanges();
        }


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetBuilder = new DatasetBuilder(context)
            {
                Title = "New Dataset",
                CreatedBy = context.Users.Where(u => u.UserId == "1").Single(),
                Category = "Test"
            };

            result = await datasetBuilder.BuildAsync();
        }


        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Dataset);

        Assert.Equal(201, result.StatusCode);

        Assert.Equal(2, result.Dataset.Id);
        Assert.Equal("New Dataset", result.Dataset.Title);
        Assert.Equal("Test", result.Dataset.Category);
        Assert.Equal(testUser.UserName, result.Dataset.CreatedBy.UserName);
    }





    [Fact]
    public async Task OnNoUser_ThrowsException()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        // Act
        using (var context = new AnnotationsDbContext(options))
        {
            var datasetBuilder = new DatasetBuilder(context)
            {
                Title = "New Dataset",
                CreatedBy = null,
                Category = "Test"
            };

            // Assert Throws
            await Assert.ThrowsAsync<ArgumentNullException>(datasetBuilder.BuildAsync);
        }
    }





    [Fact]
    public async Task OnNoTitle_BadRequest()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetBuilder = new DatasetBuilder(context)
            {
                Title = string.Empty,
                CreatedBy = new User { UserId = "1", UserName = "Test User" },
                Category = "Test"
            };

            result = await datasetBuilder.BuildAsync();

        }


        // Assert
        Assert.Equal(400, result.StatusCode);
    }





    [Fact]
    public async Task OnNoCategory_BadRequest()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;


        // Act
        ModifyDatasetResult result;

        using (var context = new AnnotationsDbContext(options))
        {
            var datasetBuilder = new DatasetBuilder(context)
            {
                Title = "New Dataset",
                CreatedBy = new User { UserId = "1", UserName = "Test User" },
                Category = string.Empty
            };

            result = await datasetBuilder.BuildAsync();
        }


        // Assert
        Assert.Equal(400, result.StatusCode);
    }
}
