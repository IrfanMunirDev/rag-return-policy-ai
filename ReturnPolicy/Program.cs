using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using ReturnPolicy.API.Middleware;
using ReturnPolicy.Infrastructure.Data;
using ReturnPolicy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:23734", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var defaultDbPath = Path.Combine(AppContext.BaseDirectory, "Data", "policy.db");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? $"Data Source={defaultDbPath}";

// If a container volume maps an external path, you can also check an env var:
var dbPathEnv = Environment.GetEnvironmentVariable("DB_PATH");
if (!string.IsNullOrEmpty(dbPathEnv))
{
    connectionString = $"Data Source={dbPathEnv}";
}
// Register EF Core SQLite
builder.Services.AddDbContext<PolicyDbContext>(options =>
   options.UseSqlite(connectionString));

// Configure Ollama AI Provider
var endpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434/v1/";
var chatModel = builder.Configuration["Ollama:ChatModel"] ?? "phi4-mini";
var embedModel = builder.Configuration["Ollama:EmbedModel"] ?? "nomic-embed-text";

var ollamaClient = new OpenAIClient(
    new System.ClientModel.ApiKeyCredential("ollama"),
    new OpenAIClientOptions { Endpoint = new Uri(endpoint) }
);

builder.Services.AddChatClient(_ => ollamaClient.GetChatClient(chatModel).AsIChatClient());
builder.Services.AddEmbeddingGenerator(_ => ollamaClient.GetEmbeddingClient(embedModel).AsIEmbeddingGenerator());

// Register RAG services from ReturnPolicy.Services library
builder.Services.AddSingleton<PolicyChunker>();
builder.Services.AddSingleton<PolicyEmbeddingService>();
builder.Services.AddSingleton<VectorSearchService>();
builder.Services.AddScoped<PolicyRagPromptStuffingService>();
builder.Services.AddScoped<PolicyRagEmbeddingService>();

var app = builder.Build();
// AUTO-CREATE DATABASE & TABLES ON STARTUP
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PolicyDbContext>();

    // Ensures directory exists and builds all tables defined in PolicyDbContext
    dbContext.Database.EnsureCreated();
}
app.UseExceptionHandler();

app.UseCors("AllowAngular");
app.UseStaticFiles();
app.MapControllers();

app.Run();