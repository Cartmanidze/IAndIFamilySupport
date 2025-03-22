using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

/// <summary>
///     Хендлер для сообщений на шаге TransferToSupport
/// </summary>
public class SupportMessageCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<SupportMessageCommandHandler> logger)
    : IRequestHandler<SupportMessageCommand, Unit>
{
    public async Task<Unit> Handle(SupportMessageCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.Chat.Id;
        var userId = request.Message.Id;
        var businessConnectionId = request.Message.BusinessConnectionId;

        var state = stateService.GetUserState(userId) ?? stateService.GetUserState(chatId);

        logger.LogInformation(
            "Пользователь {UserId} (шаг TransferToSupport) отправил сообщение: {MessageText}",
            userId, request.Message.Text
        );

        await bot.SendMessage(
            chatId,
            "Спасибо за ваше сообщение. Наш специалист обработает его и свяжется с вами в ближайшее время.",
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken
        );

        state!.CurrentStep = ScenarioStep.Start;

        return Unit.Value;
    }
}