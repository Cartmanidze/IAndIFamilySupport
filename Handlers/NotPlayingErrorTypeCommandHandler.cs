using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class NotPlayingErrorTypeCommandHandler(
    ITelegramBotClient bot,
    IFileService fileService,
    IStateService stateService)
    : IRequestHandler<NotPlayingErrorTypeCommand, Unit>
{
    public async Task<Unit> Handle(NotPlayingErrorTypeCommand request, CancellationToken cancellationToken)
    {
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);
        var data = callback.Data; // "ERROR_CODING" / "ERROR_OTHER"

        if (data == "ERROR_CODING")
        {
            if (state.SelectedDevice == "PHONE")
            {
                await bot.SendMessage(
                    chatId,
                    "Возможно, плеер(проигрыватель) в телефоне не поддерживает формат WAV. Попробуйте скачать «wav плеер», VLC, KMPlayer и т.д.",
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                await bot.SendMessage(
                    chatId,
                    "Ошибка кодировки. \"Кодировка в не поддерживаемом формате\"",
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );

                await fileService.SendPhotoAsync(bot, chatId, "EncodingError", businessConnectionId);
                await bot.SendMessage(
                    chatId,
                    "Решение: воспроизводить аудиозаписи на ОС Windows через проигрыватель Windows Media. (Нажать правой кнопкой — «Открыть с помощью» — выбрать «Windows Media»).",
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );
                await fileService.SendPhotoAsync(bot, chatId, "EncodingErrorSolution", businessConnectionId);
            }

            // Показываем FinishMenu()
            await bot.SendMessage(
                chatId,
                "Проблема решена?",
                replyMarkup: KeyboardHelper.FinishMenu(),
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.NotPlayingSolutionResult;
        }
        else if (data == "ERROR_OTHER")
        {
            // Переход в службу поддержки
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.TransferToSupport,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Неизвестный тип ошибки. Пожалуйста, выберите один из предложенных вариантов.",
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}