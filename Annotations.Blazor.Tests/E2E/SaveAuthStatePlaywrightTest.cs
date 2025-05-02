using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Annotations.Blazor.Tests.E2E;

public class SaveAuthStatePlaywrightTest
{
    [Fact]
    public async Task SaveAuthenticatedState()
    {
        string[] loginCred = await File.ReadAllLinesAsync("../../../../playwright/.auth/testUser.txt");
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var contextOptions = new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };
        var context = await browser.NewContextAsync(contextOptions); 
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:7238/authentication/login");

        await page.GetByLabel("Username or email address").FillAsync(loginCred[0]);
        
        await page.GetByLabel("Password").FillAsync(loginCred[1]);
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Log in" }).ClickAsync();
        
        await page.WaitForURLAsync("https://localhost:7238/");

        await context.StorageStateAsync(new()
        {
            Path = "../../../../playwright/.auth/orchard-state.json"
        });
    }    
    
    [Fact]
    public async Task SaveAuthenticatedStateAnnotationsUser()
    {
        string[] loginCred = await File.ReadAllLinesAsync("../../../../playwright/.auth/testUserAnnotationsUser.txt");
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var contextOptions = new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };
        var context = await browser.NewContextAsync(contextOptions); 
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:7238/authentication/login");

        await page.GetByLabel("Username or email address").FillAsync(loginCred[0]);
        
        await page.GetByLabel("Password").FillAsync(loginCred[1]);
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Log in" }).ClickAsync();
        
        await page.WaitForURLAsync("https://localhost:7238/");

        await context.StorageStateAsync(new()
        {
            Path = "../../../../playwright/.auth/orchard-state-with-AnnotationsUser.json"
        });
    }
}
