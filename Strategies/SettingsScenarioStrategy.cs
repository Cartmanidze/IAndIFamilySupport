using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Strategies;

public class SettingsScenarioStrategy : IScenarioStrategy
{
    private readonly IFileService _fileService;
    private readonly ILogger<SettingsScenarioStrategy> _logger;
    private readonly IStateService _stateService;

    public SettingsScenarioStrategy(
        IStateService stateService,
        IFileService fileService,
        ILogger<SettingsScenarioStrategy> logger)
    {
        _stateService = stateService;
        _fileService = fileService;
        _logger = logger;
    }

    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.SettingsMenu,
        ScenarioStep.VoiceActivation,
        ScenarioStep.OtherSettings
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

        await bot.SendMessage(
            chatId,
            "Пожалуйста, выберите действие из меню или напишите /start для начала заново."
        );
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;
        var data = callback.Data!;

        await bot.AnswerCallbackQuery(callback.Id);

        _logger.LogInformation("Handling settings scenario for user {UserId}, step {Step}, data {Data}",
            state.UserId, state.CurrentStep, data);

        switch (state.CurrentStep)
        {
            case ScenarioStep.SettingsMenu:
                await bot.SendMessage(
                    chatId,
                    "Основные настройки диктофона:\n" +
                    "1. Настройка качества записи\n" +
                    "2. Настройка голосовой активации\n" +
                    "3. Настройка циклической записи"
                );

                if (!string.IsNullOrEmpty(state.SelectedRecorderModel))
                    await _fileService.SendPdfInstructionAsync(bot, chatId, state.SelectedRecorderModel);
                await bot.SendMessage(
                    chatId,
                    "Удалось ли вам настроить диктофон?",
                    replyMarkup: KeyboardHelper.FinishMenu()
                );

                state.CurrentStep = ScenarioStep.Finish;
                _stateService.UpdateUserState(state);
                break;

            case ScenarioStep.VoiceActivation:
            case ScenarioStep.OtherSettings:
                await bot.SendMessage(
                    chatId,
                    "Для получения дополнительной информации, пожалуйста, обратитесь к PDF-инструкции."
                );

                if (!string.IsNullOrEmpty(state.SelectedRecorderModel))
                    await _fileService.SendPdfInstructionAsync(bot, chatId, state.SelectedRecorderModel);

                await bot.SendMessage(
                    chatId,
                    "Удалось ли вам настроить диктофон?",
                    replyMarkup: KeyboardHelper.FinishMenu()
                );

                state.CurrentStep = ScenarioStep.Finish;
                _stateService.UpdateUserState(state);
                break;

            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестный шаг сценария. Пожалуйста, напишите /start для начала заново."
                );
                break;
        }
    }
}