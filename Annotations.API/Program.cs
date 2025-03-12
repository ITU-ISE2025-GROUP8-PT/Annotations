using Annotations.API;
using Annotations.API.Groups;
using Annotations.Core.Entities;
using Microsoft.EntityFrameworkCore;
// using Azure.Storage.Blobs;
// using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Services for development using SwaggerUI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Database context service.
builder.Services.AddDbContext<AnnotationsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<AnnotationsUser>()
                .AddEntityFrameworkStores<AnnotationsDbContext>();


// Build application.
var app = builder.Build();


// Blob Storage connection string HUSK ""
//BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString); <-- indsæt string her i enden og intet andet i linjen
// Blob container name string HUSK ""
//BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName); <-- indsæt string her i enden og intet andet i linjen

app.MapGroup("/Auth").MapIdentityApi<AnnotationsUser>();

UsersGroup.MapEndpoints(app.MapGroup("/Users"));
ImagesGroup.MapEndpoints(app.MapGroup("/Images"));

app.MapGet("/Error", () => "Dette er en 400-599 eller værre");

// Development/Debugging middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline.
app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


// Application start.
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
