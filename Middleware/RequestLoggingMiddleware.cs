using System.Text;

namespace IAndIFamilySupport.API.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();

        using (var reader = new StreamReader(
                   context.Request.Body,
                   Encoding.UTF8,
                   false,
                   leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            logger.LogInformation("Получен запрос: {Body}", body);
            context.Request.Body.Position = 0;
        }

        await next(context);
    }
}