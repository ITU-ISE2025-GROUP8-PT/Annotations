using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Annotations.Blazor.Tests.E2E;

public class RBACwithAnnotationsUserTests : PageTest
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
    public async Task AccessNotDeniedDatasets()
    {
        await Page.GotoAsync("https://localhost:7238/images/datasets/");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Order by..." }).ClickAsync();
        await Page.GetByText("Manage datasets").ClickAsync();
        await Page.GetByText("Items per Page:").ClickAsync();
        await Page.GetByRole(AriaRole.Cell, new() { Name = "Dataset ID" }).ClickAsync();
        await Page.GetByRole(AriaRole.Cell, new() { Name = "No. of Images" }).ClickAsync();
        await Page.GetByRole(AriaRole.Cell, new() { Name = "Category" }).ClickAsync();
        await Page.GetByRole(AriaRole.Cell, new() { Name = "Annotated By" }).ClickAsync();
        await Page.GetByRole(AriaRole.Cell, new() { Name = "Reviewed By" }).ClickAsync();
    }    

    
    
}