namespace Annotations.API.Groups;

public static class TestGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapGet("/", () => "Hello Kitty!");

        pathBuilder.MapGet("/ShowAll", () => "Access Granted!").RequireAuthorization();

        pathBuilder.MapGet("/exception", () => 
        {
            throw new InvalidOperationException("Exception has been raised in the API. Look for further details in the log.");
        });
    }
}
