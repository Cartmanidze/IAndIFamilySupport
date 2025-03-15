using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class ConfirmPcConnectionCommandHandler
    : IRequestHandler<ConfirmPcConnectionCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IFileService _fileService;
    private readonly IStateService _stateService;

    public ConfirmPcConnectionCommandHandler(
        ITelegramBotClient bot,
        IStateService stateService,
        IFileService fileService,
        ILogger<ConfirmPcConnectionCommandHandler> logger)
    {
        _bot = bot;
        _stateService = stateService;
        _fileService = fileService;
    }

    public async Task<Unit> Handle(ConfirmPcConnectionCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        // Убираем "крутилку"
        await _bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = _stateService.GetUserState(userId);

        var data = callback.Data; // PC_CONNECTED_YES, PC_CONNECTED_NO
        if (data == "PC_CONNECTED_YES")
        {
            await _bot.SendMessage(
                chatId,
                CommonConnectionRepository.ThanksMessage,
                cancellationToken: cancellationToken
            );
            state.CurrentStep = ScenarioStep.Finish;
        }
        else if (data == "PC_CONNECTED_NO")
        {
            // Уточним модель. Если state.SelectedPhoneModel == "MACOS", то "MACOS" иначе "WINDOWS"
            var deviceModel = state.SelectedPhoneModel == "MACOS" ? "MACOS" : "WINDOWS";

            // Отправим ещё пару фото
            await _fileService.SendConnectionPhotoAsync(_bot, chatId, "PC", deviceModel, 2);
            await _fileService.SendConnectionPhotoAsync(_bot, chatId, "PC", deviceModel, 3);

            await _bot.SendMessage(
                chatId,
                CommonConnectionRepository.TransferToSupport,
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await _bot.SendMessage(
                chatId,
                "Неизвестный ответ. Пожалуйста, выберите из предложенных вариантов.",
                cancellationToken: cancellationToken
            );
        }

        _stateService.UpdateUserState(state);
        return Unit.Value;
    }
}