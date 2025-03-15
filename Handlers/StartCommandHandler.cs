using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class StartCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService) : IRequestHandler<StartCommand, Unit>
{
    public async Task<Unit> Handle(StartCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var chatId = update.Message!.Chat.Id;
        var userId = update.Message.From!.Id;

        // Достаём состояние
        var state = stateService.GetUserState(userId);

        // Логика из вашего StartScenarioStrategy:
        // Отправить приветствие, клавиатуру выбора модели, сменить шаг
        await bot.SendMessage(chatId, StartScenarioTextRepository.FirstMessageReply,
            cancellationToken: cancellationToken);
        await bot.SendMessage(
            chatId,
            StartScenarioTextRepository.ChooseModelPrompt,
            replyMarkup: KeyboardHelper.RecorderModels(),
            cancellationToken: cancellationToken
        );

        state.CurrentStep = ScenarioStep.SelectRecorderModel;
        stateService.UpdateUserState(state);

        return Unit.Value;
    }
}