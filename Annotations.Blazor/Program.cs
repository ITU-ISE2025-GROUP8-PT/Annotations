/*
 * The following code is based on https://github.com/dotnet/blazor-samples and https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
 * Provided by Microsoft Corporation under the MIT license.
 */

using Yarp.ReverseProxy.Transforms;
using Annotations.Blazor;
using Annotations.Blazor.Client.ApiTest;
using Annotations.Blazor.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

const string oidcScheme = "Annotations OIDC";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(oidcScheme)
    .AddOpenIdConnect(oidcScheme, oidcOptions =>
    {
        /* For the following OIDC settings, any line that's commented out
         * represents a DEFAULT setting. If you adopt the default, you can
         * remove the line if you wish.
         *
         * ........................................................................
         * The OIDC handler must use a sign-in scheme capable of persisting 
         * user credentials across requests.
         */

        oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        
        /* The "openid" and "profile" scopes are required for the OIDC handler 
         * and included by default. You should enable these scopes here if scopes 
         * are provided by "Authentication:Schemes:MicrosoftOidc:Scope" 
         * configuration because configuration may overwrite the scopes collection.
         */

        //oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);

        /* The following paths must match the redirect and post logout redirect 
         * paths configured when registering the application with the OIDC provider. 
         * The default values are "/signin-oidc" and "/signout-callback-oidc".
         */

        oidcOptions.CallbackPath = new PathString("/signin-oidc");
        oidcOptions.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
        
        /* The RemoteSignOutPath is the "Front-channel logout URL" for remote single 
         * sign-out. The default value is "/signout-oidc".
         */

        //oidcOptions.RemoteSignOutPath = new PathString("/signout-oidc");

        /* The authority is the server to contact for oidc authentication.
         * Here it is set to a localhost port. IRL it would refer to the website 
         * of the OpenID Connect authority.
         */
        
        oidcOptions.Authority = builder.Configuration["authentication:oidc:authority"] ?? throw new InvalidOperationException("Missing authentication:oidc:authority");
        
        /* The authority will recognise this application by the client ID. 
         * The client ID is not a secret, but it is not helpful to publish to
         * third parties. 
         * For a confidential authentication process, the application and the
         * authority will also keep a client secret. This indeed a secret and
         * must be treated as such. 
         * A client secret is only viable in cases where the server handles
         * authentication on behalf of the user. In cases where the client
         * application at the user contacts the authority itself, this scheme
         * cannot be used. 
         * To repeat, mobile/native apps and single page apps cannot use auth
         * involving a client secret!
         */

        oidcOptions.ClientId = builder.Configuration["authentication:oidc:clientid"] ?? throw new InvalidOperationException("Missing authentication:oidc:clientid");
        oidcOptions.ClientSecret = builder.Configuration["authentication:oidc:clientsecret"] ?? throw new InvalidOperationException("Missing authentication:oidc:clientsecret");

        /* Setting ResponseType to "code" configures the OIDC handler to use 
         * authorization code flow. Implicit grants and hybrid flows are unnecessary
         * in this mode. In a Microsoft Entra ID app registration, you don't need to 
         * select either box for the authorization endpoint to return access tokens 
         * or ID tokens. The OIDC handler automatically requests the appropriate 
         * tokens using the code returned from the authorization endpoint.
         */

        oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
        
        /* Set MapInboundClaims to "false" to obtain the original claim types from 
         * the token. Many OIDC servers use "name" and "role"/"roles" rather than 
         * the SOAP/WS-Fed defaults in ClaimTypes. Adjust these values if your 
         * identity provider uses different claim types.
         */

        oidcOptions.MapInboundClaims = false;
        oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
        oidcOptions.TokenValidationParameters.RoleClaimType = "role";

        ICollection<string> scopes = ["email", "roles", "profile", "api"];
        foreach (string scope in scopes) oidcOptions.Scope.Add(scope);

        /* Many OIDC providers work with the default issuer validator, but the
         * configuration must account for the issuer parameterized with "{TENANT ID}" 
         * returned by the "common" endpoint's /.well-known/openid-configuration
         * For more information, see
         * https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/1731
         */

        //var microsoftIssuerValidator = AadIssuerValidator.GetAadIssuerValidator(oidcOptions.Authority);
        //oidcOptions.TokenValidationParameters.IssuerValidator = microsoftIssuerValidator.Validate;

        /* OIDC connect options set later via ConfigureCookieOidcRefresh
         *
         * (1) The "offline_access" scope is required for the refresh token.
         *
         * (2) SaveTokens is set to true, which saves the access and refresh tokens
         * in the cookie, so the app can authenticate requests for weather data and
         * use the refresh token to obtain a new access token on access token
         * expiration.
         */
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

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
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpForwarderWithServiceDiscovery();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IApiTester, ServerApiTester>(httpClient =>
{
    httpClient.BaseAddress = new("https://localhost:7250");
});

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
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Annotations.Blazor.Client._Imports).Assembly);

app.MapForwarder("/images/APITest", "https://localhost:7250", transformBuilder =>
{
    transformBuilder.AddRequestTransform(async transformContext =>
    {
        var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
        transformContext.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);
    });
}).RequireAuthorization();

app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();
