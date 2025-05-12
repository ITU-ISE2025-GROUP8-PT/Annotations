using System;
using System.Collections.Generic;
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
}
