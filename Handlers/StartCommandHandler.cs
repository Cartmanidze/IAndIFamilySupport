using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class StartCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService) : IRequestHandler<StartCommand, Unit>
{
    public async Task<Unit> Handle(StartCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        var state = stateService.GetUserState(userId)!;

        await bot.SendMessage(chatId, StartScenarioTextRepository.FirstMessageReply,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

        await bot.SendMessage(
            chatId,
            StartScenarioTextRepository.ChooseModelPrompt,
            replyMarkup: KeyboardHelper.RecorderModels(),
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.SelectRecorderModel;
        stateService.UpdateUserState(state);

        return Unit.Value;
    }
}