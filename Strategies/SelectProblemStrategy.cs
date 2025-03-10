using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Strategies;

/// <summary>
///     Стратегия для обработки выбора проблемы подключения
/// </summary>
public class SelectProblemStrategy : IScenarioStrategy
{
    private readonly ILogger<SelectProblemStrategy> _logger;
    private readonly IStateService _stateService;

    public SelectProblemStrategy(
        IStateService stateService,
        ILogger<SelectProblemStrategy> logger)
    {
        _stateService = stateService;
        _logger = logger;
    }

    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.SelectProblem
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
        var text = message.Text?.Trim();

        switch (text)
        {
            case StartScenarioTextRepository.ProblemHowToConnect:
                await HandleConnectionProblem(bot, chatId, state);
                break;
            case StartScenarioTextRepository.ProblemNotPlaying:
                await HandlePlaybackProblem(bot, chatId, state);
                break;
            case StartScenarioTextRepository.ProblemHelpSetup:
                await HandleSettingsProblem(bot, chatId, state);
                break;
            default:
                await bot.SendMessage(
                    chatId,
                    StartScenarioTextRepository.ChooseProblemPrompt,
                    replyMarkup: KeyboardHelper.ProblemMenu()
                );
                break;
        }
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;
        var data = callback.Data!;

        await bot.AnswerCallbackQuery(callback.Id);

        _logger.LogInformation("Обработка выбора проблемы: {Data}", data);

        switch (data)
        {
            case "PROBLEM_CONNECT":
                await HandleConnectionProblem(bot, chatId, state);
                break;

            case "PROBLEM_NOTPLAY":
                await HandlePlaybackProblem(bot, chatId, state);
                break;

            case "PROBLEM_SETTINGS":
                await HandleSettingsProblem(bot, chatId, state);
                break;

            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестная проблема. Пожалуйста, выберите один из предложенных вариантов.",
                    replyMarkup: KeyboardHelper.ProblemMenu()
                );
                break;
        }
    }

    /// <summary>
    ///     Обработка выбора проблемы с подключением
    /// </summary>
    private async Task HandleConnectionProblem(ITelegramBotClient bot, long chatId, TelegramUserState state)
    {
        state.SelectedProblem = "PROBLEM_CONNECT";
        _logger.LogInformation("Пользователь выбрал проблему с подключением");

        await bot.SendMessage(
            chatId,
            ConnectionScenarioTextRepository.WhichDeviceToConnect,
            replyMarkup: KeyboardHelper.DeviceMenu()
        );

        state.CurrentStep = ScenarioStep.HowToConnectDevice;
        _stateService.UpdateUserState(state);
    }

    /// <summary>
    ///     Обработка выбора проблемы с воспроизведением записи
    /// </summary>
    private async Task HandlePlaybackProblem(ITelegramBotClient bot, long chatId, TelegramUserState state)
    {
        state.SelectedProblem = "PROBLEM_NOTPLAY";
        _logger.LogInformation("Пользователь выбрал проблему с воспроизведением");

        state.CurrentStep = ScenarioStep.NotPlayingMenu;
        _stateService.UpdateUserState(state);

        await bot.SendMessage(
            chatId,
            "Вот возможные причины, по которым запись может не воспроизводиться:\n" +
            "1. Файл записи поврежден\n" +
            "2. Диктофон не подключен правильно к устройству\n" +
            "3. У вас отсутствуют необходимые кодеки для воспроизведения"
        );
    }

    /// <summary>
    ///     Обработка выбора проблемы с настройками
    /// </summary>
    private async Task HandleSettingsProblem(ITelegramBotClient bot, long chatId, TelegramUserState state)
    {
        state.SelectedProblem = "PROBLEM_SETTINGS";
        _logger.LogInformation("Пользователь выбрал проблему с настройками");

        state.CurrentStep = ScenarioStep.SettingsMenu;
        _stateService.UpdateUserState(state);

        await bot.SendMessage(
            chatId,
            "Основные настройки диктофона:\n" +
            "1. Настройка качества записи\n" +
            "2. Настройка голосовой активации\n" +
            "3. Настройка циклической записи"
        );
    }
}