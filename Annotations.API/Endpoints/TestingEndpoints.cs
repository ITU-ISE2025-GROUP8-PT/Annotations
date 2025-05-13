namespace Annotations.API.Endpoints;

public class TestingEndpoints
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        // Anti-forgery is disabled. This was decided because the backend will not serve any forms.
        // Anti-forgery measures are covered in the front-end, and by the JWT token protection. 
        pathBuilder.RequireAuthorization().DisableAntiforgery();

        pathBuilder.MapGet("/tryme", TryMeHandler);
    }
    
    private static string[] TryMeHandler()
    {
        return ["1", "2", "3", "Dette er en prøve"];
    }
}
