using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SelectProblemSettingsCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<SelectProblemSettingsCommandHandler> logger)
    : IRequestHandler<SelectProblemSettingsCommand, Unit>
{
    public async Task<Unit> Handle(SelectProblemSettingsCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        if (chatId == 0 || userId == 0) return Unit.Value;

        var state = stateService.GetUserState(userId);

        logger.LogInformation("Пользователь {UserId} выбрал проблему с настройками", userId);
        state.SelectedProblem = "PROBLEM_SETTINGS";

        await bot.SendMessage(
            chatId,
            "Помощь в настройке",
            replyMarkup: KeyboardHelper.SettingsHelpMenu(),
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

        state.CurrentStep = ScenarioStep.SettingsMenu;
        stateService.UpdateUserState(state);

        if (request.CallbackQuery != null)
            await bot.AnswerCallbackQuery(request.CallbackQuery.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}