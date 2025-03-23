using IAndIFamilySupport.API.Handlers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Middleware;
using IAndIFamilySupport.API.Options;
using IAndIFamilySupport.API.Policies;
using IAndIFamilySupport.API.Routing;
using IAndIFamilySupport.API.Services;
using IAndIFamilySupport.API.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TelegramSettings>(
    builder.Configuration.GetSection("TelegramBot"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ConfirmRecorderCommandHandler).Assembly));

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<TelegramSettings>>();
    return new TelegramBotClient(options.Value.Token);
});

builder.Services.AddTransient<ITelegramUpdateService, TelegramUpdateService>();
builder.Services.AddSingleton<IStateService, InMemoryStateService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<IResourceService, EmbeddedResourceService>();
builder.Services.AddHostedService<DailyReportService>();

builder.Services.AddSingleton<CommandRouter>();

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