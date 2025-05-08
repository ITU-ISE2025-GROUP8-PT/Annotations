using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Annotations.Blazor.Tests.E2E;

public class RBACwithAnnotationsPlaywrightUserTests : PageTest
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
    public async Task AccessNotDeniedAnnotateOnImage()
    {
        await Page.GotoAsync("https://localhost:7238/images/annotations");
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Annotation Image" })).ToBeVisibleAsync();
    }
}
