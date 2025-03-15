using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Extensions;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

/// <summary>
///     Обработчик команды "Как подключить?" / "PROBLEM_CONNECT"
/// </summary>
public class SelectProblemConnectCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<SelectProblemConnectCommandHandler> logger)
    : IRequestHandler<SelectProblemConnectCommand, Unit>
{
    public async Task<Unit> Handle(SelectProblemConnectCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var (chatId, userId) = update.ExtractChatAndUserId();
        if (chatId == 0 || userId == 0) return Unit.Value;

        var state = stateService.GetUserState(userId);

        // Логика из HandleConnectionProblem
        logger.LogInformation("Пользователь {UserId} выбрал проблему с подключением", userId);
        state.SelectedProblem = "PROBLEM_CONNECT";

        await bot.SendMessage(
            chatId,
            ConnectionScenarioTextRepository.WhichDeviceToConnect,
            replyMarkup: KeyboardHelper.DeviceMenu(),
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.HowToConnectDevice;
        stateService.UpdateUserState(state);

        // Если была CallbackQuery, убираем "крутилку"
        if (update.CallbackQuery != null)
            await bot.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}