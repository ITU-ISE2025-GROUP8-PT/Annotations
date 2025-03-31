using System.Net.Http.Headers;
using Annotations.API;
using Annotations.API.Groups;
using Annotations.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// General services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Recipe App", Version = "v1" }); var security = new
        OpenApiSecurityScheme 
        {
            Name = HeaderNames.Authorization, 
            Type = SecuritySchemeType.ApiKey, 
            In = ParameterLocation.Header, 
            Description = "JWT Authorization header", 
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme, 
                Type = ReferenceType.SecurityScheme 
            }
        };
    x.AddSecurityDefinition(security.Reference.Id, security); 
    x.AddSecurityRequirement(new OpenApiSecurityRequirement 
        {{security, Array.Empty<string>()}}); 
});

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

// Add services to the container.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", jwtOptions =>
    {
        jwtOptions.Authority = builder.Configuration["jwt:Authority"] ?? throw new InvalidOperationException("JWT Authority not found");
        jwtOptions.Audience = builder.Configuration["jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found");;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorageConnection"]!);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorageConnection"]!);
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureStorageConnection"]!);
});
var app = builder.Build();


UsersGroup.MapEndpoints(app.MapGroup("/users"));
ImagesGroup.MapEndpoints(app.MapGroup("/images").AllowAnonymous());

app.MapGet("/error", () => "Dette er en 400-599 eller værre");

// Development/Debugging middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    InitializeTempDatabase();
}

// Middleware pipeline.
app.UseExceptionHandler("/error");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseAntiforgery();

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
        ImageData = File.ReadAllBytes("../docs/images/Perfusiontech_sampleimage.png"),
        Category = "category"
        // ImageData = await GetImageDataAsync("Perfusiontech_sampleimage.png"); <-- Eller hvad den nu kommer til at hedde når den smides op
    });
    context.SaveChanges();
}


