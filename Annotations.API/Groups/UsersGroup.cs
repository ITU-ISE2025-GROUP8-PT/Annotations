using Annotations.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.Groups;

public static class UsersGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapGet("/", () => "Hello Kitty!");
        pathBuilder.MapGet("/exception", () => 
        {
            throw new InvalidOperationException("Exception has been raised in the API. Look for further details in the log.");
        });
    }
}
