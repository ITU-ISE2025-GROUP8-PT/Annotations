namespace Annotations.API.Endpoints;


/// <summary>
/// Endpoints for testing purposes.
/// </summary>
public class TestingEndpoints
{
    /// <summary>
    /// Where all the endpoints are initialized to their respective handler.
    /// </summary>
    /// <param name="pathBuilder"></param>
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        // Anti-forgery is disabled. This was decided because the backend will not serve any forms.
        // Anti-forgery measures are covered in the front-end, and by the JWT token protection. 
        pathBuilder.RequireAuthorization().DisableAntiforgery();

        pathBuilder.MapGet("/tryme", TryMeHandler);
    }





    /// <summary>
    /// This is a test endpoint to see if the API is working.
    /// </summary>
    /// <returns></returns>
    private static string[] TryMeHandler()
    {
        return ["1", "2", "3", "Dette er en prøve"];
    }
}
