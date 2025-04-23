using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs.Models;
using Annotations.Core.Entities;
using System.Net;


namespace Annotations.API.VesselTrees;

public interface IVesselTreesBuilder
{
    /// <summary>
    /// Original filename. Will be stored as metadata. 
    /// </summary>
    int ImageId { get; set; }
    
   

    /// <summary>
    /// User uploading the file. 
    /// </summary>
    User CreatedBy { get; set; }
    
    ICollection<VesselSegment> Segments { get; set; }

    /// <summary>
    /// <para>Stores the image in the application data stores.</para>
    /// <para>This task can be executed once per instance. Fields must be correctly set.
    /// An exception is thrown if instance is set up incorrectly.</para>
    /// </summary>
    /// <returns></returns>
    Task<VesselTreeBuilderResult> BuildAsync();
}
public sealed class VesselTreeBuilderResult
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
    /// Image series entity if successfully created.
    /// </summary>
    public VesselTree? VesselTree { get; set; }
}
public class VesselTreesBuilder : IVesselTreesBuilder
{
    public int ImageId { get; set; }
    
   

    public User CreatedBy { get; set; }
    
    public ICollection<VesselSegment> Segments { get; set; }
    private readonly AnnotationsDbContext _dbContext;

    private bool buildStarted;


    public VesselTreesBuilder(AnnotationsDbContext dbContext)
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