namespace IAndIFamilySupport.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        // Разрешаем повторное чтение тела запроса
        context.Request.EnableBuffering();

        // Читаем тело запроса
        using (var reader = new StreamReader(
                   context.Request.Body, 
                   encoding: System.Text.Encoding.UTF8, 
                   detectEncodingFromByteOrderMarks: false, 
                   leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            _logger.LogInformation("Получен запрос: {Body}", body);
            // Сброс позиции потока для последующего чтения
            context.Request.Body.Position = 0;
        }
        
        await _next(context);
    }
}