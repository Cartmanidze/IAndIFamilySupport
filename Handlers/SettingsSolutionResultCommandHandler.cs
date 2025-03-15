using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SettingsSolutionResultCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService) : IRequestHandler<SettingsSolutionResultCommand, Unit>
{
    public async Task<Unit> Handle(SettingsSolutionResultCommand request, CancellationToken cancellationToken)
    {
        var callback = request.Update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);

        switch (callback.Data)
        {
            case "SETTINGS_OK":
                // Завершаем
                await bot.SendMessage(chatId,
                    "Спасибо за обращение! Если у вас будут другие вопросы — пишите. :)",
                    cancellationToken: cancellationToken);
                state.CurrentStep = ScenarioStep.Finish;
                break;

            case "SETTINGS_NOT_OK":
                // Отправляем в поддержку
                await bot.SendMessage(chatId,
                    CommonConnectionRepository.TransferToSupport,
                    cancellationToken: cancellationToken);
                state.CurrentStep = ScenarioStep.TransferToSupport;
                break;
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}