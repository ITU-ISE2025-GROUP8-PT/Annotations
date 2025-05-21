using System.Security.Claims;
using Annotations.Blazor.Client;

namespace Annotations.Blazor.Tests.Unit;

public class PersonalHomeTests : TestContext
{
    [Fact]
    public void NotAuthorizedTest()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose; // allows JS to run during bUnit tests
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("", AuthorizationState.Unauthorized); // fakes unauthorized state for a fake user
        
        // Act
        var homePage = RenderComponent<Home.Home>();
        
        // Assert
        homePage.Find("h1").MarkupMatches("<h1>Annotations - PerfusionTech</h1>");
        homePage.Find("p").MarkupMatches("<p> Log in or make a new account to get access</p>");
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
        
        authContext.SetAuthorized("FAKE TEST USER"); // fakes authorized state for a fake user
        authContext.SetClaims(claims);
        
        // Act
        var homePage = RenderComponent<Home.Home>();
        
        // Assert
        homePage.Find("h2").MarkupMatches("<h2> Welcome FAKE USER TEST</h2>");
    }

}
