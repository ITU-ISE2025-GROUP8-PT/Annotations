using Annotations.Core.Entities;
using Annotations.Core.Models;
using Annotations.Core.VesselObjects;

namespace Annotations.API.Groups;

public static class AnnotationsEndpoints
{

    public static void MapEndpoints(RouteGroupBuilder pathBuilder)
    {
        pathBuilder.RequireAuthorization();

        //This is the upload endpoint where the image first gets validated, and then gets uploaded into your local Azurite BlobStorage as a JSON file
        pathBuilder.MapPost("/annotation/save",
            async (VesselPointModel points, VesselSegmentModel segments) =>
            {
                /*Would make more sense to sent the entire VesselAnnotation through, but I am
                confused on how to do this, bc EF core wants VesselAnnotation, VesselPoint
                and VesselSegment to all be EF Core entities.
                Should we have a post/save operation for each point and each segment?
                But we need to keep track of the points and segments to do the final
                VesselAnnotation, when a tree is done.. How?*/
                
                //Would insert some logic here for saving a point to db
                
                //New class for inserting a segment to db
                
                //Another class for constructing and saving a VesselAnnotation in db
                
            });
    }
}