using System.Net.Http;
using Annotations.Blazor.Components.Pages.Annotations;
using Annotations.Blazor.Services;
using Microsoft.AspNetCore.Http;


namespace Annotations.Blazor.Tests.Unit;

public class AnnotateOnImageTests : TestContext
{

    [Fact]
    public void NotAuthorizedTest()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose; // allows JS to run during bUnit tests
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("", AuthorizationState.Unauthorized); // fakes authorized state for a fake user
        
        Services.AddSingleton<IAPIServices, APIServices>();
        Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        Services.AddHttpClient();
        
        // Act
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
        
        Services.AddSingleton<IAPIServices, APIServices>();
        Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        Services.AddHttpClient();
        
        // Act
        var annotatePage = RenderComponent<AnnotateOnImage>();
        
        // Assert
        annotatePage.Find("img").MarkupMatches("<img src=\"img/billede1.png\" alt=\"Profile Picture\" class=\"MatOverwriteProfilePic\">");
    }
}
