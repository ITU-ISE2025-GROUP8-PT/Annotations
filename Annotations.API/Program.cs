using Annotations.API;
using Annotations.API.Endpoints;
using Annotations.API.Services.Datasets;
using Annotations.API.Services.ImageAnnotation;
using Annotations.API.Services.Images;
using Annotations.API.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


// General services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var security = new OpenApiSecurityScheme
    {
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "JWT Authentication header",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(security.Reference.Id, security);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {{security, Array.Empty<string>()}});
});


builder.Services.AddControllers();


// This singleton pattern allows in-memory SQLite to work correctly.
// From: https://www.answeroverflow.com/m/1071789602316238919
builder.Services.AddSingleton(_ =>
{
    var connection = new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found"));
    connection.Open();
    return connection;
});


builder.Services.AddDbContext<AnnotationsDbContext>((serviceProvider, options) =>
{
    var connection = serviceProvider.GetRequiredService<SqliteConnection>();
    options.UseSqlite(connection);
});


// Add services to the container.
// NOTE: Both services expect the authorization header to contain "Bearer " + <the token>.
builder.Services.AddAuthentication("AnnotationsBearer")
    .AddJwtBearer()
    .AddJwtBearer("AnnotationsBearer", jwtOptions =>
    {
        jwtOptions.Authority = builder.Configuration["authentication:jwt:authority"] ?? throw new InvalidOperationException("JWT Authority not found");
        jwtOptions.Audience = builder.Configuration["authentication:jwt:audience"] ?? throw new InvalidOperationException("JWT Audience not found");
    });


// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-8.0
builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme,
        "AnnotationsBearer");
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});


builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorageConnection"]!);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorageConnection"]!);
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureStorageConnection"]!);
});


builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddTransient<IImageUploader, ImageUploader>();

builder.Services.AddScoped<IDatasetService, DatasetService>();
builder.Services.AddTransient<IDatasetBuilder, DatasetBuilder>();

builder.Services.AddScoped<IAnnotationService, AnnotationService>();

builder.Services.AddScoped<IUserService, UserService>();



var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<AnnotationsDbContext>();
    context.Database.Migrate();
}


UserEndpoints
    .MapEndpoints(app.MapGroup("/users")
    .RequireAuthorization());
AnnotationEndpoints
    .MapEndpoints(app.MapGroup("/images/annotations")
    .RequireAuthorization());
ImageEndpoints
    .MapEndpoints(app.MapGroup("/images")
    .RequireAuthorization());
DatasetEndpoints
    .MapEndpoints(app.MapGroup("/datasets")
    .RequireAuthorization());
TestingEndpoints
    .MapEndpoints(app.MapGroup("/testing")
    .RequireAuthorization());


app.MapGet("/error", () => "Exception raised in API");


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
