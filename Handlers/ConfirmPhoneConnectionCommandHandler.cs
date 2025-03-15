using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class ConfirmPhoneConnectionCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService)
    : IRequestHandler<ConfirmPhoneConnectionCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmPhoneConnectionCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);
        var data = callback.Data; // "PHONE_CONNECTED_YES" или "PHONE_CONNECTED_NO"
        if (data == "PHONE_CONNECTED_YES")
        {
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.ThanksMessage,
                cancellationToken: cancellationToken
            );
            state.CurrentStep = ScenarioStep.Finish;
        }
        else if (data == "PHONE_CONNECTED_NO")
        {
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.TransferToSupport,
                cancellationToken: cancellationToken
            );
            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Неизвестный ответ. Пожалуйста, выберите из предложенных вариантов.",
                cancellationToken: cancellationToken
            );
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}