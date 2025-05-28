using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class ConfirmRecorderCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService)
    : IRequestHandler<ConfirmRecorderCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmRecorderCommand request, CancellationToken cancellationToken)
    {
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        var state = stateService.GetUserState(userId)!;
        var previouslySelected = state.SelectedRecorderModel;
        var modelFromCallback = request.Model;

        if (previouslySelected != modelFromCallback)
        {
            await bot.SendMessage(
                chatId,
                $"Похоже, вы выбрали другую модель ({ModelHelper.GetUserFriendlyModelName(modelFromCallback)}).",
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );
            await bot.SendMessage(
                chatId,
                "Давайте уточним модель ещё раз.",
                replyMarkup: KeyboardHelper.RecorderModels(),
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.SelectRecorderModel;
        }
        else
        {
            await bot.SendMessage(
                chatId,
                $"Отлично, модель {ModelHelper.GetUserFriendlyModelName(modelFromCallback)} выбрана.",
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );

            await bot.SendMessage(
                chatId,
                StartScenarioTextRepository.ChooseProblemPrompt,
                replyMarkup: KeyboardHelper.ProblemMenu(),
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.SelectProblem;
        }

        stateService.UpdateUserState(state);

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}