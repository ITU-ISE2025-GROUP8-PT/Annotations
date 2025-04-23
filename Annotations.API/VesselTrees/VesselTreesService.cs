using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Annotations.API.Users;
using Annotations.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API.VesselTrees;

public interface IVesselTreeService
{
    /// <summary>
    /// Obtains download stream of full image from blob storage.
    /// </summary>
    /// <param name="imageId">URI for image to download.</param>
    Task<VesselTreeDownloadResult> DownloadVesselTreeAsync(int vesselTreeId);

    
}
public sealed class VesselTreeDownloadResult
{
    /// <summary>
    /// Status code for HTTP response.
    /// </summary>
    public required int StatusCode { get; set; }

    /// <summary>
    /// Error message if applicable.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Download stream. Empty stream in case of failure.
    /// </summary>
    public VesselTree? VesselTree { get; set; } 

    /// <summary>
    /// Content type. "text/plain" in case of failure.
    /// </summary>
    public string ContentType { get; set; } = "text/plain";
}

public class VesselTreesService : IVesselTreeService
{
    private readonly AnnotationsDbContext _dbContext;

    public VesselTreesService(AnnotationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VesselTreeDownloadResult> DownloadVesselTreeAsync(int vesselTreeId)
    {
        var vesselTree = await _dbContext.VesselTrees.Where(p => p.VesselTreeId == vesselTreeId)
            .Include(v => v.Segments)
                .ThenInclude(s => s.StartPoint)
            .Include(v => v.Segments)
                .ThenInclude(s => s.EndPoint)
            .SingleOrDefaultAsync();
        if (vesselTree == default(VesselTree))
            return new VesselTreeDownloadResult
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Error = "Vessel Tree not found"
            };
        return new VesselTreeDownloadResult()
        {
            StatusCode = (int) HttpStatusCode.OK,
            VesselTree = vesselTree
        };
    }
}