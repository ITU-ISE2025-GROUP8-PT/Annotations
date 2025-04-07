using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Annotations.Blazor.Tests.E2E;

public class SaveAuthStatePlaywrightTest
{
    [Fact]
    public async Task SaveAuthenticatedState()
    {
        string[] loginCred = File.ReadAllLines("../../../../playwright/.auth/testUser.txt");
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to Orchard Core login page (replace with your URL)
        await page.GotoAsync("https://localhost:7238/authentication/login");

        // Fill Orchard Core credentials (adjust selectors as needed)
        await page.GetByLabel("Username or email address").FillAsync(loginCred[0]);
        
        await page.GetByLabel("Password").FillAsync(loginCred[1]);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // Wait for post-login redirect (optional)
        await page.WaitForURLAsync("https://localhost:7238/");

        // Save state to a file (creates playwright/.auth/state.json)
        await context.StorageStateAsync(new()
        {
            Path = "../../../../playwright/.auth/orchard-state.json"
        });
    }
}