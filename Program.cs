using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Middleware;
using IAndIFamilySupport.API.Options;
using IAndIFamilySupport.API.Services;
using IAndIFamilySupport.API.Strategies;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TelegramSettings>(
    builder.Configuration.GetSection("TelegramBot"));

builder.Services.AddTransient<ITelegramUpdateService, TelegramUpdateService>();
builder.Services.AddSingleton<IStateService, InMemoryStateService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<IResourceService, EmbeddedResourceService>();

builder.Services.AddScoped<IScenarioStrategy, StartScenarioStrategy>();
builder.Services.AddScoped<IScenarioStrategy, SelectProblemStrategy>();
builder.Services.AddScoped<IScenarioStrategy, ConnectionScenarioStrategy>();
builder.Services.AddScoped<IScenarioStrategy, NotPlayingScenarioStrategy>();
builder.Services.AddScoped<IScenarioStrategy, SettingsScenarioStrategy>();
builder.Services.AddScoped<IScenarioStrategy, TransferToSupportStrategy>();
builder.Services.AddScoped<IScenarioStrategy, FinishScenarioStrategy>();

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IAndIFamilySupport API",
        Version = "v1"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramUpdateService>();
    await telegramService.SetWebhookAsync();
}

app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IAndIFamilySupport API V1"); });
}

app.MapControllers();

app.Run();