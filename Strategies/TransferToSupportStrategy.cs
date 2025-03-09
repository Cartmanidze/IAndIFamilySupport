using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Strategies;

public class TransferToSupportStrategy : IScenarioStrategy
{
    private readonly ILogger<TransferToSupportStrategy> _logger;
    private readonly IStateService _stateService;

    public TransferToSupportStrategy(
        IStateService stateService,
        ILogger<TransferToSupportStrategy> logger)
    {
        _stateService = stateService;
        _logger = logger;
    }

    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.TransferToSupport
    ];

    public async Task HandleMessageAsync(ITelegramBotClient bot, Message message, TelegramUserState state)
    {
        if (message.Text == "/start")
        {
            state.CurrentStep = ScenarioStep.Start;
            _stateService.UpdateUserState(state);
            return;
        }

        var chatId = message.Chat.Id;

        _logger.LogInformation(
            "User {UserId} in support mode sent message: {Message}",
            state.UserId, message.Text ?? "(не текстовое сообщение)");

        await bot.SendMessage(
            chatId,
            "Спасибо за ваше сообщение. Наш специалист обработает его и свяжется с вами в ближайшее время."
        );
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;
        var data = callback.Data!;

        await bot.AnswerCallbackQuery(callback.Id);

        _logger.LogInformation(
            "User {UserId} in support mode pressed button: {ButtonData}",
            state.UserId, data);

        if (data == "RESTART_BOT")
        {
            state.CurrentStep = ScenarioStep.Start;
            _stateService.UpdateUserState(state);

            await bot.SendMessage(
                chatId,
                "Начинаем заново. Отправьте /start для запуска бота."
            );
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Ваш запрос зарегистрирован. Специалист свяжется с вами в ближайшее время."
            );
        }
    }
}