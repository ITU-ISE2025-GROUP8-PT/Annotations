using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Annotations.Blazor.Tests.Annotations.Blazor.Tests.E2E;

public class AnnotationsToolPlaywrightTests: PageTest
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
    public async Task AnnotateOnImage()
    {
        await Page.GotoAsync("https://localhost:7238/images/annotations");
        await Page.GetByRole(AriaRole.Img, new() { Name = "Annotation Image" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Annotation" }).FillAsync("Artery 2mm");
        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Type" }).Locator("div").Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "Artery" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Paragraph)).ToContainTextAsync("Artery 2mm");
    }
}