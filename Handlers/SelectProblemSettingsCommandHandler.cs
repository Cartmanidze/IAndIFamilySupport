using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Extensions;
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
        var update = request.Update;
        var (chatId, userId) = update.ExtractChatAndUserId();
        if (chatId == 0 || userId == 0) return Unit.Value;

        var state = stateService.GetUserState(userId);

        logger.LogInformation("Пользователь {UserId} выбрал проблему с настройками", userId);
        state.SelectedProblem = "PROBLEM_SETTINGS";

        await bot.SendMessage(
            chatId,
            "Помощь в настройке",
            replyMarkup: KeyboardHelper.SettingsHelpMenu(),
            cancellationToken: cancellationToken);

        state.CurrentStep = ScenarioStep.SettingsMenu;
        stateService.UpdateUserState(state);

        if (update.CallbackQuery != null)
            await bot.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}