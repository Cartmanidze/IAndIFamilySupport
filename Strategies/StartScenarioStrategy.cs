using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IAndIFamilySupport.API.Strategies;

internal sealed class StartScenarioStrategy : IScenarioStrategy
{
    private readonly IFileService _fileService;
    private readonly IStateService _stateService;

    public StartScenarioStrategy(IFileService fileService, IStateService stateService)
    {
        _fileService = fileService;
        _stateService = stateService;
    }

    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.Start,
        ScenarioStep.SelectRecorderModel,
        ScenarioStep.ConfirmRecorderModel
    ];

    public async Task HandleMessageAsync(ITelegramBotClient bot, Message message, TelegramUserState state)
    {
        if (message.Type != MessageType.Text)
            return;

        var chatId = message.Chat.Id;
        var messageText = message.Text;

        switch (state.CurrentStep)
        {
            case ScenarioStep.Start:
                if (messageText == "/start")
                {
                    await bot.SendMessage(chatId, StartScenarioTextRepository.FirstMessageReply);
                    await bot.SendMessage(chatId, StartScenarioTextRepository.ChooseModelPrompt,
                        replyMarkup: KeyboardHelper.RecorderModels());
                    state.CurrentStep = ScenarioStep.SelectRecorderModel;
                    _stateService.UpdateUserState(state);
                }

                break;

            case ScenarioStep.SelectRecorderModel:
            case ScenarioStep.ConfirmRecorderModel:
                await bot.SendMessage(chatId, "Пожалуйста, выберите модель диктофона с помощью кнопок ниже.");
                await bot.SendMessage(chatId, StartScenarioTextRepository.ChooseModelPrompt,
                    replyMarkup: KeyboardHelper.RecorderModels());
                break;
        }
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;

        if (callback.Data!.StartsWith("RECORDER_"))
        {
            var model = callback.Data.Replace("RECORDER_", "");
            state.SelectedRecorderModel = model;

            await bot.SendMessage(chatId, $"Выбрана модель диктофона: {ModelHelper.GetUserFriendlyModelName(model)}");
            await _fileService.SendModelPhotoAsync(bot, chatId, model);

            await bot.SendMessage(chatId,
                $"Подтвердите, пожалуйста, что это точно ваша модель {ModelHelper.GetUserFriendlyModelName(model)}.",
                replyMarkup: KeyboardHelper.ConfirmMenu($"CONFIRM_RECORDER_{model}", "RESELECT"));

            state.CurrentStep = ScenarioStep.ConfirmRecorderModel;
            _stateService.UpdateUserState(state);
        }
        else if (callback.Data == "RESELECT")
        {
            await bot.SendMessage(chatId, "Хорошо, выберите другую модель диктофона.",
                replyMarkup: KeyboardHelper.RecorderModels());
            state.CurrentStep = ScenarioStep.SelectRecorderModel;
            _stateService.UpdateUserState(state);
        }
        else if (callback.Data!.StartsWith("CONFIRM_RECORDER_"))
        {
            var model = callback.Data.Replace("CONFIRM_RECORDER_", "");

            if (state.SelectedRecorderModel != model)
            {
                await bot.SendMessage(chatId,
                    $"Похоже, вы выбрали другую модель ({ModelHelper.GetUserFriendlyModelName(model)}).");
                await bot.SendMessage(chatId, "Давайте уточним модель ещё раз.",
                    replyMarkup: KeyboardHelper.RecorderModels());
                state.CurrentStep = ScenarioStep.SelectRecorderModel;
            }
            else
            {
                await bot.SendMessage(chatId,
                    $"Отлично, модель {ModelHelper.GetUserFriendlyModelName(model)} выбрана.");

                await _fileService.SendPdfInstructionAsync(bot, chatId, model);

                await bot.SendMessage(chatId, StartScenarioTextRepository.ChooseProblemPrompt,
                    replyMarkup: KeyboardHelper.ProblemMenu());

                state.CurrentStep = ScenarioStep.SelectProblem;
            }

            _stateService.UpdateUserState(state);
        }
        else if (callback.Data!.StartsWith("PDF_"))
        {
            var model = callback.Data.Replace("PDF_", "");
            await _fileService.SendPdfInstructionAsync(bot, chatId, model);
        }

        await bot.AnswerCallbackQuery(callback.Id);
    }
}