/*
 * The following code is based on https://github.com/dotnet/blazor-samples and https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcServer
 * Provided by Microsoft Corporation under the MIT license.
 */

using Annotations.Blazor;
using Annotations.Blazor.Components;
using Microsoft.AspNetCore.Authentication.Cookies;


const string oidcScheme = "Annotations OIDC";

var builder = WebApplication.CreateBuilder(args);

/*
 * Configures authentication in the program including the connection
 * to OICD. For more information see the AuthenticationServiceCollectionExtensions
 */
builder.Services.AddAuthenticationServiceCollection(builder.Configuration);

/* ConfigureCookieOidcRefresh attaches a cookie OnValidatePrincipal callback to get
 * a new access token when the current one expires, and reissue a cookie with the
 * new access token saved inside. If the refresh fails, the user will be signed
 * out. OIDC connect options are set for saving tokens and the offline access
 * scope.
 */
builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, oidcScheme);

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGroup("/authentication").MapLoginAndLogout();
app.MapGroup("/images");

app.Run();
