using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Annotations.Blazor.Tests.E2E;

public class UserDataPlaywrightTest : PageTest
{
    
    private const string AuthStatePath = "../../../../playwright/.auth/orchard-state-with-AnnotationsUser.json";
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            StorageStatePath = AuthStatePath
        };
    }
    
    [Fact]
    public async Task UserInfoOnPersonalHomePageTest()
    {
        await Page.GotoAsync("https://localhost:7238/");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Welcome" }).ClickAsync();
        await Page.GetByText("Personal Information testUserAnnotationsUser AnnotationsUser").ClickAsync();

    }
    
    [Fact]
    public async Task UserInfoOnAnnotationsPageTest()
    {
        await Page.GotoAsync("https://localhost:7238/images/annotations");
        await Page.GetByText("testUserAnnotationsUser").ClickAsync();
        await Page.GetByText("AnnotationsUser", new() { Exact = true }).ClickAsync();
        
    }
    
    
}