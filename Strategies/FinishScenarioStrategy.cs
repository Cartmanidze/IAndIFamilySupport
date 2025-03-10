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
                CommonConnectionRepository.ThanksMessage
            );
        }
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;
        var data = callback.Data!;

        await bot.AnswerCallbackQuery(callback.Id);

        _logger.LogInformation("Handling finish scenario callback: {Data}", data);

        switch (data)
        {
            case "PROBLEM_SOLVED":
                await bot.SendMessage(
                    chatId,
                    "Мы рады, что смогли помочь вам решить проблему! " +
                    CommonConnectionRepository.ThanksMessage
                );
                break;
            case "PROBLEM_OTHER":
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.TransferToSupport
                );

                state.CurrentStep = ScenarioStep.TransferToSupport;
                _stateService.UpdateUserState(state);
                break;
            default:
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
                break;
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