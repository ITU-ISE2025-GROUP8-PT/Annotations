/*
 * https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/
 * This has been the go to for this setup.
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Annotations.Core.Entities;
using Annotations.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Annotations.API.Groups;

public class AccountsGroup
{
    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.MapPost("/register", async ([FromBody] RegisterModel model, UserManager<AnnotationsUser> userManager) =>
        {
            var newUser = new AnnotationsUser { UserName = model.Email, Email = model.Email };

            var result = await userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return new RegisterResult { Successful = false, Errors = errors };

            }
            return new RegisterResult { Successful = true };
        });
        
        pathBuilder.MapPost("/login", async ([FromBody] LoginRequest model, SignInManager<AnnotationsUser> signInManager, IConfiguration configuration) =>
        {
            var user = await signInManager.UserManager.FindByNameAsync(model.Email);
            if (user == null) return Results.BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });

            var result = await signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (!result.Succeeded) return Results.BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });

            var claims = await signInManager.ClaimsFactory.CreateAsync(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityKey"] ?? throw new InvalidOperationException()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                configuration["JwtIssuer"],
                configuration["JwtAudience"],
                claims.Claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Results.Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        });
    }
}
