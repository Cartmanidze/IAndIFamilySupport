using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SelectPcModelCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    IFileService fileService,
    ILogger<SelectPcModelCommandHandler> logger)
    : IRequestHandler<SelectPcModelCommand, Unit>
{
    public async Task<Unit> Handle(SelectPcModelCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);

        var data = callback.Data; // "PC_WINDOWS" или "PC_MACOS"
        var pcModel = data!.StartsWith("PC_") ? data[3..] : data;

        // Отправим фото шаги подключения к PC
        await fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel);
        await fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 2);
        await fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 3);

        await bot.SendMessage(
            chatId,
            ConnectionScenarioTextRepository.DidConnectToComputer,
            replyMarkup: KeyboardHelper.YesNoMenu("PC_CONNECTED_YES", "PC_CONNECTED_NO"),
            cancellationToken: cancellationToken
        );

        // Переходим к подтверждению
        state.CurrentStep = ScenarioStep.ConfirmPcConnected;
        stateService.UpdateUserState(state);

        logger.LogInformation("Пользователь {UserId} выбрал модель ПК: {PcModel}", userId, pcModel);

        return Unit.Value;
    }
}