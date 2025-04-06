using Annotations.Blazor.Components.Pages.Annotations;

namespace Annotations.Blazor.Tests.Annotations.Blazor.Tests.Unit;

public class AnnotateOnImageTests : TestContext
{

    [Fact]
    public void NotAuthorizedTest()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose; // allows JS to run during bUnit tests
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("", AuthorizationState.Unauthorized); // fakes authorized state for a fake user
        var annotatePage = RenderComponent<AnnotateOnImage>();
        
        // Assert
        annotatePage.Find("h1").MarkupMatches("<h1>Annotations - PerfusionTech</h1>");
        annotatePage.Find("p").MarkupMatches("<p> Log in or make a new account to get access</p>");
    }
    
    [Fact]
    public void AuthorizedTest()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose; // allows JS to run during bUnit tests
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("FAKE USER TEST"); // fakes authorized state for a fake user
        var annotatePage = RenderComponent<AnnotateOnImage>();
        
        // Assert
        annotatePage.Find("img").MarkupMatches("<img src=\"img/billede1.png\" alt=\"Profile Picture\" style=\"width: 80px; height: 80px; border-radius: 50%; object-fit: cover;\">");
        
    }
}