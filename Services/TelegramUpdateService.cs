using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Options;
using IAndIFamilySupport.API.States;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IAndIFamilySupport.API.Services;

public class TelegramUpdateService : ITelegramUpdateService
{
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<TelegramUpdateService> _logger;
    private readonly TelegramSettings _settings;
    private readonly IStateService _stateService;
    private readonly IEnumerable<IScenarioStrategy> _strategies;

    public TelegramUpdateService(
        IOptions<TelegramSettings> options,
        ILogger<TelegramUpdateService> logger,
        IStateService stateService,
        IEnumerable<IScenarioStrategy> strategies)
    {
        _settings = options.Value;
        _logger = logger;
        _stateService = stateService;
        _strategies = strategies;

        _botClient = new TelegramBotClient(_settings.Token);
    }

    public async Task SetWebhookAsync()
    {
        _logger.LogInformation("Устанавливаем Webhook на {WebhookUrl}", _settings.WebhookUrl);
        await _botClient.SetWebhook(_settings.WebhookUrl);
    }

    public async Task HandleUpdateAsync(Update update)
    {
        long userId;

        TelegramUserState? state = null;

        switch (update.Type)
        {
            case UpdateType.Message when update.Message?.From != null:
                userId = update.Message.From.Id;
                if (update.Message.Text == "/start")
                {
                    state = new TelegramUserState
                    {
                        CurrentStep = ScenarioStep.Start
                    };
                    _stateService.UpdateUserState(state);
                    return;
                }

                break;

            case UpdateType.CallbackQuery when update.CallbackQuery?.From != null:
                userId = update.CallbackQuery.From.Id;
                break;

            default:
                _logger.LogWarning("Неподдерживаемый тип обновления: {UpdateType}", update.Type);
                return;
        }

        state ??= _stateService.GetUserState(userId);

        var targetStep = state.CurrentStep;

        IScenarioStrategy? strategy = null;

        if (strategy == null)
        {
            strategy = _strategies.FirstOrDefault(s => s.TargetSteps.Contains(state.CurrentStep));

            if (strategy != null)
                _logger.LogInformation("Выбрана стратегия для текущего шага {Step}", state.CurrentStep);
        }

        if (strategy == null)
        {
            _logger.LogWarning("Не найдена стратегия для шага {Step} или целевого шага {TargetStep}",
                state.CurrentStep, targetStep);

            state.CurrentStep = ScenarioStep.Start;
            _stateService.UpdateUserState(state);

            strategy = _strategies.FirstOrDefault(s => s.TargetSteps.Contains(state.CurrentStep));

            if (strategy == null)
            {
                _logger.LogError("Критическая ошибка: не найдена стратегия для начального шага");
                return;
            }
        }

        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await strategy.HandleMessageAsync(_botClient, update.Message!, state);
                    break;

                case UpdateType.CallbackQuery:
                    await strategy.HandleCallbackAsync(_botClient, update.CallbackQuery!, state);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке обновления для пользователя {UserId} на шаге {Step}",
                userId, state.CurrentStep);

            try
            {
                var chatId = update.Type == UpdateType.Message
                    ? update.Message!.Chat.Id
                    : update.CallbackQuery!.Message!.Chat.Id;

                await _botClient.SendMessage(
                    chatId,
                    "Произошла ошибка при обработке запроса. Пожалуйста, попробуйте еще раз или напишите /start для начала заново.");
            }
            catch (Exception innerEx)
            {
                _logger.LogError(innerEx, "Ошибка при отправке сообщения об ошибке пользователю {UserId}", userId);
            }
        }
    }
}