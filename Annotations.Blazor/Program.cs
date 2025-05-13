/*
 * The following code is based on https://github.com/dotnet/blazor-samples and https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcServer
 * Provided by Microsoft Corporation under the MIT license.
 */

using Annotations.Blazor;
using Annotations.Blazor.Components;
using Annotations.Blazor.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Yarp.ReverseProxy.Transforms;


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
builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpForwarder();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddHttpClient();
//builder.Services.AddScoped<IAPIServices, APIServices>();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Annotations.Blazor.Client._Imports).Assembly);


app.MapForwarder("/api/{**remainder}", "https://localhost:7250/", transformBuilder =>
{
    transformBuilder.AddRequestTransform(async transformContext =>
    {
        var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
        transformContext.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);

        // Preserve the original request path
        var requestPath = transformContext.HttpContext.Request.RouteValues["remainder"]?.ToString() ?? string.Empty;

        // Preserve query string parameters
        var queryString = transformContext.HttpContext.Request.QueryString.Value ?? string.Empty;

        transformContext.ProxyRequest.RequestUri = new Uri($"https://localhost:7250/{requestPath}{queryString}");
    });
}).RequireAuthorization();


app.MapGroup("/authentication").MapLoginAndLogout();


app.Run();
