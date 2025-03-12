using Annotations.API;
using Annotations.API.Groups;
using Annotations.Core.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
// using Azure.Storage.Blobs;
// using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// General services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AnnotationsDbContext>(options => 
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Blob Storage connection string HUSK ""
//BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString); <-- indsæt string her i enden og intet andet i linjen
// Blob container name string HUSK ""
//BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName); <-- indsæt string her i enden og intet andet i linjen

UsersGroup.MapEndpoints(app.MapGroup("/users"));
ImagesGroup.MapEndpoints(app.MapGroup("/images"));

app.MapGet("/error", () => "Dette er en 400-599 eller værre");

// Development/Debugging middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline.
app.UseExceptionHandler("/error");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseHttpsRedirection();

app.Run();



/*
// Helper metode til at hive billeder ud som Byte[] fra blob
async Task<byte[]> GetImageDataAsync(string blobName)
{
    BlobClient blobClient = containerClient.GetBlobClient(blobName);
    using var memoryStream = new MemoryStream();
    await blobClient.DownloadToAsync(memoryStream);
    return memoryStream.ToArray();
}
*/
