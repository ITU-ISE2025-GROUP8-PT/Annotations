using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Annotations.API.Services.Users;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace Annotations.API.Tests;


public class UserServiceTests
{
    [Fact]
    public async Task GetUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Users.Add(new User { UserId = "1", UserName = "Alice" });
            context.Users.Add(new User { UserId = "2", UserName = "Bob" });
            context.SaveChanges();
        }

        // Act
        List<UserModel> result;

        using (var context = new AnnotationsDbContext(options))
        {
            var userService = new UserService(context);
            result = await userService.GetUsersAsync();
        }

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.UserName == "Alice");
        Assert.Contains(result, u => u.UserName == "Bob");
    }





    [Fact]
    public async Task TryFindUserAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Users.Add(new User { UserId = "1", UserName = "Alice" });
            context.SaveChanges();
        }

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Alice")
        }));

        // Act
        User? result;

        using (var context = new AnnotationsDbContext(options))
        {
            var userService = new UserService(context);
            result = await userService.TryFindUserAsync(claimsPrincipal);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.UserName);
    }





    [Fact]
    public async Task TryFindUserAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Users.Add(new User { UserId = "1", UserName = "Alice" });
            context.SaveChanges();
        }

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "2"),
            new Claim(ClaimTypes.Name, "Bob")
        }));

        // Act
        User? result;

        using (var context = new AnnotationsDbContext(options))
        {
            var userService = new UserService(context);
            result = await userService.TryFindUserAsync(claimsPrincipal);
        }

        // Assert
        Assert.Null(result);
    }




    [Fact]
    public async Task CreateUserAsync_UserDoesNotExist_CreatesUser()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;
        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
        }
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Alice")
        }));
        // Act
        User result;
        using (var context = new AnnotationsDbContext(options))
        {
            var userService = new UserService(context);
            result = await userService.CreateUserAsync(claimsPrincipal);
        }
        // Assert
        Assert.NotNull(result);
        Assert.Equal("1", result.UserId);
        Assert.Equal("Alice", result.UserName);
    }




    [Fact]
    public async Task CreateUserAsync_UserAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AnnotationsDbContext>()
            .UseSqlite(connection)
            .Options;
        using (var context = new AnnotationsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Users.Add(new User { UserId = "1", UserName = "Alice" });
            context.SaveChanges();
        }
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Alice")
        }));
        // Act & Assert
        using (var context = new AnnotationsDbContext(options))
        {
            var userService = new UserService(context);
            await Assert.ThrowsAsync<InvalidOperationException>(() => userService.CreateUserAsync(claimsPrincipal));
        }
    }
}
