using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Interfaces;

public interface ITelegramUpdateService
{
    Task SetWebhookAsync();

    Task HandleUpdateAsync(Update update);
}