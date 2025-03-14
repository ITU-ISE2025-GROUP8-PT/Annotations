using Annotations.API;
using Annotations.API.Groups;
using Annotations.Core.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
// using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// General services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Temporary SQLite based database service.
// This singleton pattern allows in-memory SQLite to work correctly.
// From: https://www.answeroverflow.com/m/1071789602316238919
builder.Services.AddSingleton(_ =>
{
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();
    return connection;
});

builder.Services.AddDbContext<AnnotationsDbContext>((serviceProvider, options) =>
{
    var connection = serviceProvider.GetRequiredService<SqliteConnection>();
    options.UseSqlite(connection);
});


var app = builder.Build();

// Blob Storage connection string HUSK ""
//BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString); <-- indsæt string her i enden og intet andet i linjen
// Blob container name string HUSK ""
//BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName); <-- indsæt string her i enden og intet andet i linjen
//var connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02x...;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
var connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

// Test blob .txt upload
BlobClient blobClient = containerClient.GetBlobClient("test.txt");
await blobClient.UploadAsync(
    new MemoryStream("Hello, Azurite!"u8.ToArray()), 
    overwrite: true
);
Console.WriteLine("Uploaded and potentially overwrite 'test.txt' to Azurite.");

// Test blob .txt download
var memoryStream = new MemoryStream();
await blobClient.DownloadToAsync(memoryStream);
string content = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
Console.WriteLine($"Downloaded content: {content}");

// Test blob image file upload
var imagePath = "../docs/images/Perfusiontech_sampleimage.png";
BlobClient imageBlobClient = containerClient.GetBlobClient("Perfusiontech_sampleimage.png");
using (var fileStream = File.OpenRead(imagePath))
{
    await imageBlobClient.UploadAsync(fileStream, overwrite: true);
    Console.WriteLine("Uploaded 'Perfusiontech_sampleimage.png' to Azurite.");
}



UsersGroup.MapEndpoints(app.MapGroup("/users"));
ImagesGroup.MapEndpoints(app.MapGroup("/images"));

app.MapGet("/error", () => "Dette er en 400-599 eller værre");

// Development/Debugging middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    InitializeTempDatabase();
}

// Middleware pipeline.
app.UseExceptionHandler("/error");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseHttpsRedirection();

app.Run();




// Helper to initialize a database within developer environment.
void InitializeTempDatabase()
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<AnnotationsDbContext>();
    context.Database.Migrate();

    context.Add(new Admin
    {
        UserId = 0,
        FirstName = "Admin",
        LastName = "Adminsen",
        Email = "admin@adminsen.com"
    }); 
    context.Add(new MedicalProfessional
    {
        UserId = 1,
        FirstName = "Medical",
        LastName = "Professional",
        Email = "med@prof.com",
        Affiliation = "Rigshospitalet",
        JobTitle = "Surgeon",
        TotalAssignmentsFinished = 0,
        ProfilePictureId = 123
    });
    context.Add(new Image
    {
        Id = 1,
        Title = "Sample Image",
        Description = "This is a sample image.",
        ImageData = File.ReadAllBytes("../docs/images/Perfusiontech_sampleimage.png")
        // ImageData = await GetImageDataAsync("Perfusiontech_sampleimage.png"); <-- Eller hvad den nu kommer til at hedde når den smides op
    });
    context.SaveChanges();
}

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
