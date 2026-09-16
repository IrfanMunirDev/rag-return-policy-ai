using Microsoft.Extensions.AI;
using OpenAI;
using ReturnPolicy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Read provider selection from appsettings.json (e.g., "Ollama" or "OpenAI")
var aiProvider = builder.Configuration["AIProvider"] ?? "Ollama";

if (aiProvider.Equals("Ollama", StringComparison.OrdinalIgnoreCase))
{
    // Register local Ollama via OpenAI-compatible endpoint
    var endpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434/v1/";
    var model = builder.Configuration["Ollama:Model"] ?? "phi4-mini";

    builder.Services.AddChatClient(_ => new OpenAIClient(
        new System.ClientModel.ApiKeyCredential("ollama"),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint) }
    ).GetChatClient(model).AsIChatClient());
}
else
{
    // Register paid OpenAI API
    var apiKey = builder.Configuration["OpenAI:ApiKey"]
                 ?? throw new InvalidOperationException("API key missing.");

    builder.Services.AddChatClient(_ => new OpenAIClient(apiKey)
        .GetChatClient("gpt-4o-mini").AsIChatClient());
}

builder.Services.AddSingleton<PolicyService>();

var app = builder.Build();
app.UseStaticFiles();
app.MapControllers();

app.Run();
