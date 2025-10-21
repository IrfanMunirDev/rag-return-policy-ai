using ReturnPolicy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<PolicyService>();

var app = builder.Build();
app.UseStaticFiles();
app.MapControllers();

app.Run();
