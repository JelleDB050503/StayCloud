using BookingService.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// 👉Cosmos DB instellingen ophalen uit appsettings.json
var cosmosDbSection = builder.Configuration.GetSection("CosmosDb");
string account = cosmosDbSection["Account"]!;
string key = cosmosDbSection["Key"]!;
string databaseName = cosmosDbSection["DatabaseName"]!;
string containerName = cosmosDbSection["ContainerName"]!;


// ✅ CosmosDbService toevoegen via Dependency Injection
builder.Services.AddSingleton<ICosmosDbService>(serviceProvider =>
{
    var client = new CosmosClient(account, key);
    return new CosmosDbService(client, databaseName, containerName);
});


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
