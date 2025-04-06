using Annotations.Blazor.Components.Pages;

namespace Annotations.Blazor.Tests.Annotations.Blazor.Tests.Unit;

public class AnnotateOnImageTests : TestContext
{

    [Fact]
    public void NotAuthorizedTest()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose; // allows JS to run during bUnit tests
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("FAKE TEST USER"); // fakes authorized state for a fake user
        var homePage = RenderComponent<Home>();
    }
}