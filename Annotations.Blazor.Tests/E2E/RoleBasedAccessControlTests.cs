using System.Threading.Tasks;
using Microsoft.Playwright.Xunit;
using Microsoft.Playwright;

namespace Annotations.Blazor.Tests.E2E;

public class RoleBasedAccessControlTests : PageTest
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
        await Page.GetByRole(AriaRole.Heading, new() { Name = "You are not welcome here!" }).ClickAsync();
        await Page.GetByRole(AriaRole.Img, new() { Name = "Access Denied" }).ClickAsync();
    }
    
    
}