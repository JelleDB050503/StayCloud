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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BookingService",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Voer hieronder je JWT-token in (beginnend met 'Bearer ')"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddHttpClient();

// 👉Cosmos DB instellingen ophalen uit appsettings.json
var cosmosDbSection = builder.Configuration.GetSection("CosmosDb");
string account = cosmosDbSection["Account"]!;
string key = cosmosDbSection["Key"]!;
string databaseName = cosmosDbSection["DatabaseName"]!;
string containerName = cosmosDbSection["ContainerName"]!;


//  CosmosDbService toevoegen via Dependency Injection
builder.Services.AddSingleton<ICosmosDbService>(serviceProvider =>
{
    var client = new CosmosClient(account, key);
    return new CosmosDbService(client, databaseName, containerName);
});

// JWT settings uit appsettings ophalen
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Voor eenvoud
        ValidateAudience = false, // Voor eenvoud
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddAuthorization(); // Nodig voor [Authorize]

// blob storage ophalen uit appsettings
var blobStorageSection = builder.Configuration.GetSection("BlobStorage");
string blobConnectionString = blobStorageSection["ConnectionString"]!;
string blobContainerName = blobStorageSection["ContainerName"]!;

builder.Services.AddSingleton<IBlobStorageService>(new BlobStorageService(blobConnectionString, blobContainerName));
builder.Services.AddSingleton<IFileProcessor, FileProcessor>();
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var emailSection = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailSection);
builder.Services.AddScoped<IEmailService, EmailService>();





var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
