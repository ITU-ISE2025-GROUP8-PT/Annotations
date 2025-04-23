using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Annotations.API.Users;
using Annotations.Core.Entities;

namespace Annotations.API.VesselTrees;

public static class VesselTreesEndpoint
{
    public static void MapEndpoints(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization().DisableAntiforgery();
        groupBuilder.MapPost("/Upload", NewVesselTreeHandler);
        groupBuilder.MapGet("/Download/{vesselTreeId}", GetVesselTreeHandler);
        
    }
    static async Task<VesselTreeBuilderResult> NewVesselTreeHandler(
        ICollection<VesselSegment> Segments,
        int imageId,
        long?           addToSeries, // TODO: Allow upload into existing image series directly.
        ClaimsPrincipal claimsPrincipal,
        HttpContext     httpContext,
        [FromServices] IVesselTreesBuilder builder,
        [FromServices] IUserService   userService
    )
    {
        var user = await userService.TryFindUserAsync(claimsPrincipal) ?? await userService.CreateUser(claimsPrincipal);

        builder.ImageId = imageId;
        builder.CreatedBy = user;
        builder.Segments = Segments;
        

        var uploaderResult = await builder.BuildAsync();

        httpContext.Response.StatusCode = uploaderResult.StatusCode;
        return uploaderResult;
    }
    
    static async Task<VesselTree?> GetVesselTreeHandler(
        [FromRoute] int vesselTreeId,
        HttpContext httpContext,
        [FromServices] IVesselTreeService vesselTreeService
    )
    {
        var vesselTreeResult = await vesselTreeService.DownloadVesselTreeAsync(vesselTreeId);

        httpContext.Response.StatusCode = vesselTreeResult.StatusCode;
        return vesselTreeResult.VesselTree;
    }
}