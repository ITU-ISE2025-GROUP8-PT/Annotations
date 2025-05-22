using Annotations.Blazor.Client.Pages.AnnotationTools;
using Annotations.Blazor.Client.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Annotations.Blazor.Client;


namespace Annotations.Blazor.Tests.Unit;

public class AnnotateOnImageTests : TestContext
{

    [Fact]
    public void NotAuthorizedTest()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose; // allows JS to run during bUnit tests
        var authContext = this.AddTestAuthorization();
        
        var claims = new[]
        {
            new Claim(UserInfo.UserIdClaimType, "test-user-id"),
            new Claim(UserInfo.NameClaimType, "FAKE USER TEST"),
            new Claim(UserInfo.RoleClaimType, "test-role")
        };
        
        authContext.SetAuthorized("", AuthorizationState.Unauthorized); // fakes authorized state for a fake user
        authContext.SetClaims(claims);
        
        Services.AddSingleton<IAnnotationDataAccessor, AnnotationDataAccessor>();
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
        var claims = new[]
        {
            new Claim(UserInfo.UserIdClaimType, "test-user-id"),
            new Claim(UserInfo.NameClaimType, "FAKE USER TEST"),
            new Claim(UserInfo.RoleClaimType, "test-role")
        };
        authContext.SetAuthorized("FAKE USER TEST");
        authContext.SetClaims(claims);// fakes authorized state for a fake user
        
        Services.AddSingleton<IAnnotationDataAccessor, AnnotationDataAccessor>();
        Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        Services.AddHttpClient();
        
        // Act
        var annotatePage = RenderComponent<AnnotateOnImage>();
        
        // Assert
        annotatePage.Find("img").MarkupMatches("<img src=\"img/billede1.png\" alt=\"Profile Picture\" class=\"MatOverwriteProfilePic\">");
    }
}
