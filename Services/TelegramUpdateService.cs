using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IAndIFamilySupport.API.Services;

public class TelegramUpdateService : ITelegramUpdateService
{
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<TelegramUpdateService> _logger;
    private readonly TelegramSettings _settings;

    public TelegramUpdateService(
        IOptions<TelegramSettings> options,
        ILogger<TelegramUpdateService> logger)
    {
        _settings = options.Value;
        _logger = logger;

        _botClient = new TelegramBotClient(_settings.Token);
    }

    /// <summary>
    ///     Устанавливаем Webhook на URL, указанный в конфигурации.
    ///     Вызывается при старте приложения (в Program.cs).
    /// </summary>
    public async Task SetWebhookAsync()
    {
        _logger.LogInformation("Устанавливаем Webhook на {WebhookUrl}", _settings.WebhookUrl);
        await _botClient.SetWebhook(_settings.WebhookUrl);
    }

    /// <summary>
    ///     Основной метод для обработки входящих обновлений (Update).
    ///     Вызывается из контроллера при POST-запросе от Telegram.
    /// </summary>
    /// <param name="update"></param>
    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Type == UpdateType.Message)
            await HandleMessage(update.Message!);
        else if (update.Type == UpdateType.CallbackQuery) await HandleCallbackQuery(update.CallbackQuery!);
        // можно расширять на другие типы Update
    }

    private async Task HandleMessage(Message message)
    {
        if (message.Type != MessageType.Text)
            return;

        var chatId = message.Chat.Id;
        var text = message.Text;

        _logger.LogInformation("Получено текстовое сообщение: {Text} из чата {ChatId}", text, chatId);

        if (text == "/start")
            await SendWelcomeMenu(chatId);
        else
            // Любое другое сообщение
            await _botClient.SendMessage(
                chatId,
                "Пожалуйста, выберите действие через /start или используйте меню.",
                replyMarkup: new ReplyKeyboardRemove()
            );
    }

    private async Task HandleCallbackQuery(CallbackQuery callbackQuery)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var data = callbackQuery.Data;

        _logger.LogInformation("Обработка нажатия кнопки: {Data}", data);

        switch (data)
        {
            case "R8_PLUS":
            case "R3":
            case "R8":
                await SendIssueMenu(chatId, data);
                break;

            case "HOW_TO_CONNECT":
                await _botClient.SendMessage(
                    chatId,
                    "Инструкция по подключению:\n1) ...\n2) ...\n3) ..."
                );
                break;

            case "NO_PLAY":
                await _botClient.SendMessage(
                    chatId,
                    "Причины, по которым не воспроизводится запись:\n- ...\n- ..."
                );
                break;

            case "SETTINGS_HELP":
                await _botClient.SendMessage(
                    chatId,
                    "Помощь в настройке:\n1) ...\n2) ..."
                );
                break;

            default:
                await _botClient.SendMessage(chatId, "Неизвестная команда");
                break;
        }

        await _botClient.AnswerCallbackQuery(callbackQuery.Id);
    }

    private async Task SendIssueMenu(long chatId, string model)
    {
        var text = $"Вы выбрали модель: {model}\nВыберите проблему:";

        var inlineKeyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("Как подключить?", "HOW_TO_CONNECT")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Не воспроизводится?", "NO_PLAY")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Помощь в настройке", "SETTINGS_HELP")
            ]
        ]);

        await _botClient.SendMessage(
            chatId,
            text,
            replyMarkup: inlineKeyboard
        );
    }

    /// <summary>
    ///     Первое меню (выбор диктофона).
    /// </summary>
    private async Task SendWelcomeMenu(long chatId)
    {
        var welcomeText =
            "Здравствуйте!\n\n" +
            "Добро пожаловать в службу техподдержки I and I family!\n" +
            "Опишите, пожалуйста, вашу проблему, или выберите устройство ниже.\n\n" +
            "Также можем предложить посмотреть наш Telegram-канал: https://t.me/IandIfamily";

        var inlineKeyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("R8 PLUS (8,32,64)", "R8_PLUS"),
                InlineKeyboardButton.WithCallbackData("R3 (8,32,64)", "R3"),
                InlineKeyboardButton.WithCallbackData("R8 (8,32,64)", "R8")
            ]
        ]);

        await _botClient.SendMessage(
            chatId,
            welcomeText,
            replyMarkup: inlineKeyboard
        );
    }
}