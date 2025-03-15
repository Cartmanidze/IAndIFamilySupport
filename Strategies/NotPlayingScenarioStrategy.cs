using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace IAndIFamilySupport.API.Strategies;

public class NotPlayingScenarioStrategy : IScenarioStrategy
{
    private readonly IFileService _fileService;
    private readonly ILogger<NotPlayingScenarioStrategy> _logger;
    private readonly IStateService _stateService;

    public NotPlayingScenarioStrategy(IStateService stateService, IFileService fileService,
        ILogger<NotPlayingScenarioStrategy> logger)
    {
        _stateService = stateService;
        _fileService = fileService;
        _logger = logger;
    }

    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.NotPlayingMenu,
        ScenarioStep.NotPlayingDeviceSelection,
        ScenarioStep.NotPlayingErrorType,
        ScenarioStep.NotPlayingSolutionResult
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

        _logger.LogInformation("Handling not playing scenario for user {UserId}, step {Step}, data {Data}",
            state.UserId, state.CurrentStep, data);

        switch (state.CurrentStep)
        {
            case ScenarioStep.NotPlayingMenu:
                await HandleDeviceSelection(bot, chatId, data, state);
                break;

            case ScenarioStep.NotPlayingErrorType:
                await HandleErrorType(bot, chatId, data, state);
                break;

            case ScenarioStep.NotPlayingSolutionResult:
                await HandleSolutionResult(bot, chatId, data, state);
                break;

            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестный шаг сценария. Пожалуйста, напишите /start для начала заново."
                );
                break;
        }
    }

    private async Task HandleDeviceSelection(ITelegramBotClient bot, long chatId, string data, TelegramUserState state)
    {
        switch (data)
        {
            case "PLAYBACK_PHONE":
                state.SelectedDevice = "PHONE";
                await HandlePlayback(bot, chatId, state);
                break;

            case "PLAYBACK_PC":
                state.SelectedDevice = "PC";
                await HandlePlayback(bot, chatId, state);
                break;

            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестное устройство. Пожалуйста, выберите из предложенных вариантов.",
                    replyMarkup: KeyboardHelper.PlaybackDeviceMenu()
                );
                break;
        }
    }

    private async Task HandlePlayback(ITelegramBotClient bot, long chatId, TelegramUserState state)
    {
        var keyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("Ошибка кодировки", "ERROR_CODING")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Другая ошибка", "ERROR_OTHER")
            ]
        ]);

        await bot.SendMessage(
            chatId,
            "Какую ошибку вы получаете?",
            replyMarkup: keyboard
        );

        state.CurrentStep = ScenarioStep.NotPlayingErrorType;
        _stateService.UpdateUserState(state);
    }

    private async Task HandleErrorType(ITelegramBotClient bot, long chatId, string data, TelegramUserState state)
    {
        if (data == "ERROR_CODING")
        {
            if (state.SelectedDevice == "PHONE")
            {
                await bot.SendMessage(
                    chatId,
                    "Возможно, плеер(проигрыватель) в телефоне не поддерживает формат WAV. Для прослушивания попробуйте скачать такие плееры «wav плеер», Медиаплеер VLC, KMPlayer (в гугл плей)"
                );
            }
            else
            {
                await bot.SendMessage(
                    chatId,
                    "Ошибка кодировки. \"Кодировка в не поддерживаемом формате\""
                );


                await _fileService.SendPhotoAsync(bot, chatId, "EncodingError");

                await bot.SendMessage(
                    chatId,
                    "Решение: Воспроизводить аудиозаписи на ОС WINDOWS, необходимо через проигрыватель WINDOWS MEDIA. (Что бы выбрать этот проигрыватель необходимо: нажать правой кнопкой мыши на нужную запись => нажать на пункт «открыть с помощью» и среди вариантов выбрать «проигрыватель windows media», так же может называться «традиционный проигрыватель windows media») + фото"
                );

                await _fileService.SendPhotoAsync(bot, chatId,
                    "EncodingErrorSolution");
            }

            await bot.SendMessage(
                chatId,
                "Проблема решена?",
                replyMarkup: KeyboardHelper.FinishMenu()
            );

            state.CurrentStep = ScenarioStep.NotPlayingSolutionResult;
            _stateService.UpdateUserState(state);
        }
        else if (data == "ERROR_OTHER")
        {
            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
            _stateService.UpdateUserState(state);
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Неизвестный тип ошибки. Пожалуйста, выберите один из предложенных вариантов."
            );
        }
    }

    private async Task HandleSolutionResult(ITelegramBotClient bot, long chatId, string data, TelegramUserState state)
    {
        if (data == "PROBLEM_SOLVED")
        {
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.ThanksMessage
            );

            state.CurrentStep = ScenarioStep.Finish;
            _stateService.UpdateUserState(state);
        }
        else if (data == "PROBLEM_OTHER")
        {
            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
            _stateService.UpdateUserState(state);
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Неизвестный выбор. Пожалуйста, выберите один из предложенных вариантов.",
                replyMarkup: KeyboardHelper.FinishMenu()
            );
        }
    }
}