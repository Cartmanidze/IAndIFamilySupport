using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Strategies;

internal sealed class StartScenarioStrategy : IScenarioStrategy
{
    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.Start,
        ScenarioStep.SelectRecorderModel,
        ScenarioStep.ConfirmRecorderModel
    ];

    public Task HandleMessageAsync(ITelegramBotClient bot, Message message, TelegramUserState state)
    {
        throw new NotImplementedException();
    }

    public Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        throw new NotImplementedException();
    }
}