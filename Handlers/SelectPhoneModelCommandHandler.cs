using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SelectPhoneModelCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    IFileService fileService,
    ILogger<SelectPhoneModelCommandHandler> logger)
    : IRequestHandler<SelectPhoneModelCommand, Unit>
{
    public async Task<Unit> Handle(SelectPhoneModelCommand request, CancellationToken cancellationToken)
    {
        var update = request.Update;
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        // Убираем "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);

        var data = callback.Data; // например "PHONE_SAMSUNG", "PHONE_IPHONE_NEW"
        var phoneModel = data!.StartsWith("PHONE_") ? data[6..] : data;
        state.SelectedPhoneModel = phoneModel;

        logger.LogInformation("Пользователь {UserId} выбрал модель телефона: {PhoneModel}", userId, phoneModel);

        if (phoneModel == "NOT_LISTED")
        {
            // Не нашли модель
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.TransferToSupport,
                cancellationToken: cancellationToken
            );
            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            // Отправляем фото подключения
            await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel);

            // Если iPhone_old, дополнительно сообщим про OTG
            if (phoneModel == "IPHONE_OLD")
                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.NeededOtg,
                    cancellationToken: cancellationToken);

            if (phoneModel is "IPHONE_OLD" or "IPHONE_NEW")
            {
                // Дополнительные фото
                await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 2);
                await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 3);
                await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 4);

                if (phoneModel == "IPHONE_OLD")
                    await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 5);

                await bot.SendMessage(chatId, AccessoriesCheckRepository.CheckAccessories,
                    cancellationToken: cancellationToken);

                await fileService.SendVideoAsync(bot, chatId, "PHONE", phoneModel);
            }
            else
            {
                // Android-модели
                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.OtgActivationRecommendation,
                    cancellationToken: cancellationToken);
                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.GetOtgActivationInstruction(phoneModel),
                    cancellationToken: cancellationToken);
            }

            await bot.SendMessage(chatId, ConnectionScenarioTextRepository.DictaphoneFolderLocation,
                cancellationToken: cancellationToken);

            // Спросим, получилось ли
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.DidConnectToPhone,
                replyMarkup: KeyboardHelper.YesNoMenu("PHONE_CONNECTED_YES", "PHONE_CONNECTED_NO"),
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.ConfirmPhoneConnected;
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}