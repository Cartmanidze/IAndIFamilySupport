using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class NotPlayingSelectDeviceCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<NotPlayingSelectDeviceCommandHandler> logger)
    : IRequestHandler<NotPlayingSelectDeviceCommand, Unit>
{
    public async Task<Unit> Handle(NotPlayingSelectDeviceCommand request, CancellationToken cancellationToken)
    {
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId)!;
        var data = callback.Data; // "PLAYBACK_PHONE" или "PLAYBACK_PC"
        switch (data)
        {
            case "PLAYBACK_PHONE":
                state.SelectedDevice = "PHONE";
                break;
            case "PLAYBACK_PC":
                state.SelectedDevice = "PC";
                break;
        }

        logger.LogInformation("User {UserId} selected device {Device}", userId, state.SelectedDevice);

        await bot.SendMessage(
            chatId,
            "Какую ошибку вы получаете?",
            replyMarkup: KeyboardHelper.PlayingError(),
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken
        );

        // Переходим к шагу NotPlayingErrorType
        state.CurrentStep = ScenarioStep.NotPlayingErrorType;
        stateService.UpdateUserState(state);

        return Unit.Value;
    }
}