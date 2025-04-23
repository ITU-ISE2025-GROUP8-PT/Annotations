using Annotations.Core.Entities;
using System.Net;

namespace Annotations.API.VesselTrees;

public interface IVesselTreesBuilder
{
    int ImageId { get; set; }
    
    User CreatedBy { get; set; }
    
    ICollection<VesselSegment> Segments { get; set; }
    
    Task<VesselTreeBuilderResult> BuildAsync();
}

public sealed class VesselTreeBuilderResult
{
    public required int StatusCode { get; set; }
    
    public string Error { get; set; } = string.Empty;
    
    public VesselTree? VesselTree { get; set; }
}

public class VesselTreeBuilder : IVesselTreesBuilder
{
    public int ImageId { get; set; }
    public User CreatedBy { get; set; }
    
    public ICollection<VesselSegment> Segments { get; set; }
    private readonly AnnotationsDbContext _dbContext;

    private bool buildStarted;

    public VesselTreeBuilder(AnnotationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<VesselTreeBuilderResult> BuildAsync()
    {
        if (buildStarted)
        {
            throw new InvalidOperationException("Operation was already started.");
        }
        buildStarted = true;

        var vesselTree = await CreateInDatabaseAndReturn();

        return new VesselTreeBuilderResult()
        {
            StatusCode = (int)HttpStatusCode.Created,
            VesselTree = vesselTree
        };
    }
    
    private async Task<VesselTree> CreateInDatabaseAndReturn()
    {
        if (CreatedBy == null) throw new NullReferenceException(nameof(CreatedBy));

        var vesselTreeEntity = new VesselTree()
        {
            ImageId = ImageId,
            Segments = Segments,
            CreatedBy = CreatedBy
        };

        await _dbContext.AddAsync(vesselTreeEntity);
        await _dbContext.SaveChangesAsync();

        return vesselTreeEntity;
    }
}
