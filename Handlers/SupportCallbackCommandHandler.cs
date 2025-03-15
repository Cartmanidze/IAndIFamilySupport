using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SupportCallbackCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<SupportCallbackCommandHandler> logger)
    : IRequestHandler<SupportCallbackCommand, Unit>
{
    public async Task<Unit> Handle(SupportCallbackCommand request, CancellationToken cancellationToken)
    {
        var callback = request.Update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);
        if (state.CurrentStep != ScenarioStep.TransferToSupport)
            // Если не в поддержке, игнорируем или что-то отвечаем
            return Unit.Value;

        var data = callback.Data;

        logger.LogInformation(
            "Пользователь {UserId} в режиме поддержки нажал кнопку: {ButtonData}",
            userId, data
        );

        if (data == "RESTART_BOT")
        {
            state.CurrentStep = ScenarioStep.Start;
            stateService.UpdateUserState(state);

            await bot.SendMessage(
                chatId,
                "Начинаем заново. Отправьте /start для запуска бота.",
                cancellationToken: cancellationToken
            );
        }
        else
        {
            // Любой другой коллбэк
            await bot.SendMessage(
                chatId,
                "Ваш запрос зарегистрирован. Специалист свяжется с вами в ближайшее время.",
                cancellationToken: cancellationToken
            );
        }

        state.CurrentStep = ScenarioStep.Start;

        return Unit.Value;
    }
}