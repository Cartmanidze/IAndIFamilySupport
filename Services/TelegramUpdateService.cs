using IAndIFamilySupport.API.Extensions;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Options;
using IAndIFamilySupport.API.Routing;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IAndIFamilySupport.API.Services;

public class TelegramUpdateService(
    ILogger<TelegramUpdateService> logger,
    CommandRouter router,
    IMediator mediator,
    IOptions<TelegramSettings> settings,
    ITelegramBotClient botClient)
    : ITelegramUpdateService
{
    private readonly TelegramSettings _settings = settings.Value;

    public async Task SetWebhookAsync()
    {
        logger.LogInformation("Устанавливаем Webhook на {WebhookUrl}", _settings.WebhookUrl);
        await botClient.SetWebhook(_settings.WebhookUrl);
    }

    public async Task HandleUpdateAsync(Update update)
    {
        var user = update.ExtractUser();

        if (user?.Id == 6313458815)
            return;

        var command = router.ResolveCommand(update);
        if (command == null)
        {
            logger.LogInformation(
                "Не найдена команда или команда была отправлена от лица аккаунта к которому привязан бот для UpdateType={Type}",
                update.Type);
            await SendUnknownCommandMessage(update);
            return;
        }

        try
        {
            // Отправляем в MediatR -> он найдёт нужный Handler
            await mediator.Send(command);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке команды.");

            switch (update.Type)
            {
                case UpdateType.Message when update.Message != null:
                    await botClient.SendMessage(update.Message.Chat.Id,
                        "Произошла ошибка. Попробуйте /start или обратитесь в поддержку.",
                        businessConnectionId: update.Message.BusinessConnectionId);
                    break;
                case UpdateType.BusinessMessage when update.BusinessMessage != null:
                    await botClient.SendMessage(update.BusinessMessage.Chat.Id,
                        "Произошла ошибка. Попробуйте /start или обратитесь в поддержку.",
                        businessConnectionId: update.BusinessMessage.BusinessConnectionId);
                    break;
                case UpdateType.CallbackQuery when update.CallbackQuery?.Message != null:
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat.Id,
                        "Произошла ошибка. Попробуйте /start или обратитесь в поддержку.",
                        businessConnectionId: update.CallbackQuery.Message.BusinessConnectionId);
                    break;
            }
        }
    }

    /// <summary>
    ///     Отправляем сообщение пользователю, что команда не распознана.
    /// </summary>
    private async Task SendUnknownCommandMessage(Update update)
    {
        try
        {
            long? chatId = null;
            string? businessConnectionId = null;

            switch (update.Type)
            {
                case UpdateType.Message when update.Message != null:
                    chatId = update.Message.Chat.Id;
                    businessConnectionId = update.Message?.BusinessConnectionId;
                    break;

                case UpdateType.BusinessMessage when update.BusinessMessage != null:
                    chatId = update.BusinessMessage.Chat.Id;
                    businessConnectionId = update.BusinessMessage?.BusinessConnectionId;
                    break;

                case UpdateType.CallbackQuery when update.CallbackQuery?.Message != null:
                    chatId = update.CallbackQuery.Message.Chat.Id;
                    businessConnectionId = update.CallbackQuery.Message?.BusinessConnectionId;
                    break;
            }

            if (chatId.HasValue)
                await botClient.SendMessage(
                    chatId.Value,
                    "Извините, я не понял вашу команду. Попробуйте /start или обратитесь в поддержку.",
                    businessConnectionId: businessConnectionId
                );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при отправке ответа о неизвестной команде");
        }
    }
}