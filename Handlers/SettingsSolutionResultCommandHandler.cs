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
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);

        switch (callback.Data)
        {
            case "SETTINGS_OK":
                await bot.SendMessage(chatId,
                    "Спасибо за обращение! Если у вас будут другие вопросы — пишите. :)",
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);
                state.CurrentStep = ScenarioStep.Finish;
                break;

            case "SETTINGS_NOT_OK":
                await bot.SendMessage(chatId,
                    CommonConnectionRepository.TransferToSupport,
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);
                state.CurrentStep = ScenarioStep.TransferToSupport;
                break;
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}