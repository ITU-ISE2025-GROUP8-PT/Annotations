using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Annotations.Blazor.Tests.Annotations.Blazor.Tests.E2E;

public class AnnotationsToolPlaywrightTests: PageTest
{
    [Fact]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }

    [Fact]
    public async Task GetStartedLink()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Get started" }).ClickAsync();

        // Expects page to have a heading with the name of Installation.
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" })).ToBeVisibleAsync();
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