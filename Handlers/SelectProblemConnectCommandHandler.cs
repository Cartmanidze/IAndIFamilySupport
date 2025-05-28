using IAndIFamilySupport.API.Commands;
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
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();
        if (chatId == 0 || userId == 0) return Unit.Value;

        var state = stateService.GetUserState(userId)!;

        // Логика из HandleConnectionProblem
        logger.LogInformation("Пользователь {UserId} выбрал проблему с подключением", userId);
        state.SelectedProblem = "PROBLEM_CONNECT";

        await bot.SendMessage(
            chatId,
            ConnectionScenarioTextRepository.WhichDeviceToConnect,
            replyMarkup: KeyboardHelper.DeviceMenu(),
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.HowToConnectDevice;
        stateService.UpdateUserState(state);

        // Если была CallbackQuery, убираем "крутилку"
        if (request.CallbackQuery != null)
            await bot.AnswerCallbackQuery(request.CallbackQuery.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}