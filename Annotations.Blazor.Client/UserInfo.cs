/*
 * The following code is based on https://github.com/dotnet/blazor-samples and https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
 * Provided by Microsoft Corporation under the MIT license.
 */

using System.Security.Claims;

namespace Annotations.Blazor.Client;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public required string Role { get; set; }

    public const string UserIdClaimType = "sub";
    public const string NameClaimType = "name";
    public const string RoleClaimType = "role";

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
        new()
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType),
            Role = GetRequiredClaim(principal, RoleClaimType),
        };

    public ClaimsPrincipal ToClaimsPrincipal() =>
        new(new ClaimsIdentity(
            [new(UserIdClaimType, UserId), new(NameClaimType, Name), new(RoleClaimType, Role)],
            authenticationType: nameof(UserInfo),
            nameType: NameClaimType,
            roleType: RoleClaimType));

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
}
