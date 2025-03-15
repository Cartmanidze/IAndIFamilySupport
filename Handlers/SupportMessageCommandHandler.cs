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
        var update = request.Update;
        var message = update.Message!;
        var chatId = message.Chat.Id;
        var userId = message.From?.Id ?? 0;

        // Проверяем, действительно ли пользователь находится на TransferToSupport
        var state = stateService.GetUserState(userId);

        // Логируем сообщение
        logger.LogInformation(
            "Пользователь {UserId} (шаг TransferToSupport) отправил сообщение: {MessageText}",
            userId, message.Text
        );

        // Отвечаем
        await bot.SendMessage(
            chatId,
            "Спасибо за ваше сообщение. Наш специалист обработает его и свяжется с вами в ближайшее время.",
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.Start;

        return Unit.Value;
    }
}