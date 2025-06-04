using BookingService.Services.Auth;
using BookingService.Services;
using Microsoft.Azure.Cosmos;
using BookingService.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Swagger configuratie
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingService", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Voer je JWT-token in (startend met 'Bearer ')"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// JWT Authenticatie
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://staycloud-identity-jdb.azurewebsites.net";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(); // Nodig voor [Authorize]

// CosmosDB
var cosmosDbSection = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<ICosmosDbService>(serviceProvider =>
{
    var client = new CosmosClient(cosmosDbSection["Account"], cosmosDbSection["Key"]);
    return new CosmosDbService(client, cosmosDbSection["DatabaseName"], cosmosDbSection["ContainerName"]);
});

// Blob Storage
var blobSection = builder.Configuration.GetSection("BlobStorage");
builder.Services.AddSingleton<IBlobStorageService>(
    new BlobStorageService(blobSection["ConnectionString"], blobSection["ContainerName"])
);

// Overige services
builder.Services.AddSingleton<IFileProcessor, FileProcessor>();
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<PriceServiceClient>();

// ----------------------
//      MIDDLEWARE
// ----------------------

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
