using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Services;

public interface ITelegramUpdateService
{
    Task SetWebhookAsync();
    Task HandleUpdateAsync(Update update);
}