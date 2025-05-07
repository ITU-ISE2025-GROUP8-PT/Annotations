using System.Threading.Tasks;
using Microsoft.Playwright.Xunit;
using Microsoft.Playwright;

namespace Annotations.Blazor.Tests.E2E;

public class RoleBasedAccessControlPlaywrightTests : PageTest
{
    private const string AuthStatePath = "../../../../playwright/.auth/orchard-state.json";
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            StorageStatePath = AuthStatePath
        };
    }
    
    [Fact]
    public async Task AccessDeniedDatasets()
    {
        await Page.GotoAsync("https://localhost:7238/images/datasets/");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Unfortunately you do not have the required permission to access this site." })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Access Denied" })).ToBeVisibleAsync();
    }    
    
    [Fact]
    public async Task AccessDeniedImageAnnotations()
    {
        await Page.GotoAsync("https://localhost:7238/images/annotations");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Unfortunately you do not have the required permission to access this site." })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Access Denied" })).ToBeVisibleAsync();
    }    
    
    [Fact]
    public async Task AccessDeniedApiAccess()
    {
        await Page.GotoAsync("https://localhost:7238/ApiAccess");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Unfortunately you do not have the required permission to access this site." })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Access Denied" })).ToBeVisibleAsync();
    }
}
