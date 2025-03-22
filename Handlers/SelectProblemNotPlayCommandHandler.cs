using IAndIFamilySupport.API.Commands;
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
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();
        if (chatId == 0 || userId == 0) return Unit.Value;

        var state = stateService.GetUserState(userId);

        logger.LogInformation("Пользователь {UserId} выбрал проблему с воспроизведением", userId);
        state.SelectedProblem = "PROBLEM_NOTPLAY";

        await bot.SendMessage(
            chatId,
            "Где вы пытаетесь воспроизвести запись?",
            replyMarkup: KeyboardHelper.PlaybackDeviceMenu(),
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.NotPlayingMenu;
        stateService.UpdateUserState(state);

        // Закрываем "крутилку", если это был callback
        if (request.CallbackQuery != null)
            await bot.AnswerCallbackQuery(request.CallbackQuery.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}