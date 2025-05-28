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
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId)!;

        var data = callback.Data; // "PC_WINDOWS" или "PC_MACOS"
        var pcModel = data!.StartsWith("PC_") ? data[3..] : data;

        // Отправим фото шаги подключения к PC
        await fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 1, businessConnectionId);
        await fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 2, businessConnectionId);
        await fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 3, businessConnectionId);

        await bot.SendMessage(
            chatId,
            ConnectionScenarioTextRepository.DidConnectToComputer,
            replyMarkup: KeyboardHelper.YesNoMenu("PC_CONNECTED_YES", "PC_CONNECTED_NO"),
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken
        );

        // Переходим к подтверждению
        state.CurrentStep = ScenarioStep.ConfirmPcConnected;
        stateService.UpdateUserState(state);

        logger.LogInformation("Пользователь {UserId} выбрал модель ПК: {PcModel}", userId, pcModel);

        return Unit.Value;
    }
}