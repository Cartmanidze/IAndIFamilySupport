using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

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
        ScenarioStep.NotPlayingMenu
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

        await bot.AnswerCallbackQuery(callback.Id);

        _logger.LogInformation("Handling not playing scenario for user {UserId}", state.UserId);

        await bot.SendMessage(
            chatId,
            "Вот возможные причины, по которым запись может не воспроизводиться:\n" +
            "1. Файл записи поврежден\n" +
            "2. Диктофон не подключен правильно к устройству\n" +
            "3. У вас отсутствуют необходимые кодеки для воспроизведения"
        );

        await _fileService.SendPhotoAsync(bot, chatId, "CONNECTION_PHOTO");

        await _fileService.SendPhotoAsync(bot, chatId, "FILES_FOLDER_PHOTO");

        await bot.SendMessage(
            chatId,
            "Вам удалось решить проблему?",
            replyMarkup: KeyboardHelper.FinishMenu()
        );

        // Update state to finish
        state.CurrentStep = ScenarioStep.Finish;
        _stateService.UpdateUserState(state);
    }
}