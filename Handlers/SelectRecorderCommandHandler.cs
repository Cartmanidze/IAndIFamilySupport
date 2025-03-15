using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SelectRecorderCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    IFileService fileService)
    : IRequestHandler<SelectRecorderCommand, Unit>
{
    public async Task<Unit> Handle(SelectRecorderCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        // Вот тут "Model" берётся из группы "?<model>" из RegEx @"^RECORDER_(?<model>.+)$"
        var selectedModel = request.Model;

        // Обновляем состояние
        var state = stateService.GetUserState(userId);
        state.SelectedRecorderModel = selectedModel;
        state.CurrentStep = ScenarioStep.ConfirmRecorderModel;
        stateService.UpdateUserState(state);

        // Сообщаем пользователю
        await bot.SendMessage(chatId,
            $"Выбрана модель диктофона: {ModelHelper.GetUserFriendlyModelName(selectedModel)}",
            cancellationToken: cancellationToken);

        // Отправляем фото
        await fileService.SendModelPhotoAsync(bot, chatId, selectedModel);

        // Предлагаем подтвердить
        await bot.SendMessage(
            chatId,
            $"Подтвердите, что это точно ваша модель {ModelHelper.GetUserFriendlyModelName(selectedModel)}.",
            replyMarkup: KeyboardHelper.ConfirmMenu($"CONFIRM_RECORDER_{selectedModel}", "RESELECT"),
            cancellationToken: cancellationToken
        );

        // Ответим на коллбек, чтобы убрать "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}