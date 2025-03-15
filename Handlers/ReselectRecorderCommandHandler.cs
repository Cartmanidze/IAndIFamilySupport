using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class ReselectRecorderCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService) : IRequestHandler<ReselectRecorderCommand, Unit>
{
    public async Task<Unit> Handle(ReselectRecorderCommand request, CancellationToken cancellationToken)
    {
        var callback = request.Update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        var state = stateService.GetUserState(userId);
        state.CurrentStep = ScenarioStep.SelectRecorderModel;
        stateService.UpdateUserState(state);

        await bot.SendMessage(
            chatId,
            "Хорошо, выберите другую модель диктофона.",
            replyMarkup: KeyboardHelper.RecorderModels(),
            cancellationToken: cancellationToken
        );

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}