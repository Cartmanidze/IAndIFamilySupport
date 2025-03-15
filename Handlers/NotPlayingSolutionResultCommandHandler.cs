using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class NotPlayingSolutionResultCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService)
    : IRequestHandler<NotPlayingSolutionResultCommand, Unit>
{
    public async Task<Unit> Handle(NotPlayingSolutionResultCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);

        var data = callback.Data; // "PROBLEM_SOLVED" / "PROBLEM_OTHER"
        switch (data)
        {
            case "PROBLEM_SOLVED":
                await bot.SendMessage(chatId, CommonConnectionRepository.ThanksMessage,
                    cancellationToken: cancellationToken);
                state.CurrentStep = ScenarioStep.Finish;
                break;
            case "PROBLEM_OTHER":
                await bot.SendMessage(chatId, CommonConnectionRepository.TransferToSupport,
                    cancellationToken: cancellationToken);
                state.CurrentStep = ScenarioStep.TransferToSupport;
                break;
            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестный выбор. Пожалуйста, выберите один из предложенных вариантов.",
                    replyMarkup: KeyboardHelper.FinishMenu(),
                    cancellationToken: cancellationToken
                );
                break;
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}