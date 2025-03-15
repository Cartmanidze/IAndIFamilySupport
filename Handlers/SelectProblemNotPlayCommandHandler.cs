using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Extensions;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SelectProblemNotPlayCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<SelectProblemNotPlayCommandHandler> logger)
    : IRequestHandler<SelectProblemNotPlayCommand, Unit>
{
    public async Task<Unit> Handle(SelectProblemNotPlayCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var (chatId, userId) = update.ExtractChatAndUserId();
        if (chatId == 0 || userId == 0) return Unit.Value;

        var state = stateService.GetUserState(userId);

        logger.LogInformation("Пользователь {UserId} выбрал проблему с воспроизведением", userId);
        state.SelectedProblem = "PROBLEM_NOTPLAY";

        await bot.SendMessage(
            chatId,
            "Где вы пытаетесь воспроизвести запись?",
            replyMarkup: KeyboardHelper.PlaybackDeviceMenu(),
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.NotPlayingMenu;
        stateService.UpdateUserState(state);

        // Закрываем "крутилку", если это был callback
        if (update.CallbackQuery != null)
            await bot.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}