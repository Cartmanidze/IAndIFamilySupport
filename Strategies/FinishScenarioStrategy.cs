using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Strategies;

public class FinishScenarioStrategy : IScenarioStrategy
{
    private readonly ILogger<FinishScenarioStrategy> _logger;
    private readonly IStateService _stateService;

    public FinishScenarioStrategy(
        IStateService stateService,
        ILogger<FinishScenarioStrategy> logger)
    {
        _stateService = stateService;
        _logger = logger;
    }

    public IEnumerable<ScenarioStep> TargetSteps => new[]
    {
        ScenarioStep.Finish
    };

    public async Task HandleMessageAsync(ITelegramBotClient bot, Message message, TelegramUserState state)
    {
        var chatId = message.Chat.Id;

        if (message.Text == "/start")
        {
            ResetState(state);
            _stateService.UpdateUserState(state);

            await bot.SendMessage(
                chatId,
                "Начинаем заново..."
            );

            await bot.SendMessage(
                chatId,
                StartScenarioTextRepository.FirstMessageReply
            );

            await bot.SendMessage(
                chatId,
                StartScenarioTextRepository.ChooseModelPrompt,
                replyMarkup: KeyboardHelper.RecorderModels()
            );

            state.CurrentStep = ScenarioStep.SelectRecorderModel;
            _stateService.UpdateUserState(state);
        }
        else
        {
            await bot.SendMessage(
                chatId,
                CommonPhoneConnectionRepository.ThanksMessage
            );
        }
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;
        var data = callback.Data!;

        await bot.AnswerCallbackQuery(callback.Id);

        _logger.LogInformation("Handling finish scenario callback: {Data}", data);

        if (data == "PROBLEM_SOLVED")
        {
            await bot.SendMessage(
                chatId,
                "Мы рады, что смогли помочь вам решить проблему! " +
                CommonPhoneConnectionRepository.ThanksMessage
            );
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
            ResetState(state);
            _stateService.UpdateUserState(state);

            await bot.SendMessage(
                chatId,
                "Начинаем заново..."
            );

            await bot.SendMessage(
                chatId,
                StartScenarioTextRepository.FirstMessageReply
            );

            await bot.SendMessage(
                chatId,
                StartScenarioTextRepository.ChooseModelPrompt,
                replyMarkup: KeyboardHelper.RecorderModels()
            );

            state.CurrentStep = ScenarioStep.SelectRecorderModel;
            _stateService.UpdateUserState(state);
        }
    }

    private void ResetState(TelegramUserState state)
    {
        state.CurrentStep = ScenarioStep.Start;
        state.SelectedDevice = null;
        state.SelectedPhoneModel = null;
        state.SelectedProblem = null;
        state.SelectedRecorderModel = null;
    }
}