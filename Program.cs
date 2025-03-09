using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Middleware;
using IAndIFamilySupport.API.Options;
using IAndIFamilySupport.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Читаем настройки из appsettings.json
builder.Services.Configure<TelegramSettings>(
    builder.Configuration.GetSection("TelegramBot"));

// Подключаем сервисы
builder.Services.AddTransient<ITelegramUpdateService, TelegramUpdateService>();

builder.Services.AddSingleton<IStateService, InMemoryStateService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy());

// 1) Регистрируем сервисы для Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Опционально: описываем базовую информацию для UI
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IAndIFamilySupport API",
        Version = "v1"
    });
});

// Билдим приложение
var app = builder.Build();

// При запуске приложения настраиваем Webhook
using (var scope = app.Services.CreateScope())
{
    var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramUpdateService>();
    await telegramService.SetWebhookAsync();
}

// Регистрируем middleware для логирования запроса
app.UseMiddleware<RequestLoggingMiddleware>();

// 2) Подключаем Swagger middleware
//    - Генерирует JSON-эндпоинт по умолчанию /swagger/v1/swagger.json
//    - Подключает UI /swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IAndIFamilySupport API V1"); });
}

// Подключаем маршрутизацию для контроллеров
app.MapControllers();

// Запуск приложения
app.Run();