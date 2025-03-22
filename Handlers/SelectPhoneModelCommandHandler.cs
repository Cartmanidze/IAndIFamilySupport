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
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

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
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );
            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            // Отправляем фото подключения
            await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 1, businessConnectionId);

            // Если iPhone_old, дополнительно сообщим про OTG
            if (phoneModel == "IPHONE_OLD")
                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.NeededOtg,
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);

            if (phoneModel is "IPHONE_OLD" or "IPHONE_NEW")
            {
                // Дополнительные фото
                await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 2, businessConnectionId);
                await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 3, businessConnectionId);
                await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 4, businessConnectionId);

                if (phoneModel == "IPHONE_OLD")
                    await fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel, 5,
                        businessConnectionId);

                await bot.SendMessage(chatId, AccessoriesCheckRepository.CheckAccessories,
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);

                await fileService.SendVideoAsync(bot, chatId, "PHONE", phoneModel, businessConnectionId);
            }
            else
            {
                // Android-модели
                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.OtgActivationRecommendation,
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);
                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.GetOtgActivationInstruction(phoneModel),
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);
            }

            await bot.SendMessage(chatId, ConnectionScenarioTextRepository.DictaphoneFolderLocation,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken);

            // Спросим, получилось ли
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.DidConnectToPhone,
                replyMarkup: KeyboardHelper.YesNoMenu("PHONE_CONNECTED_YES", "PHONE_CONNECTED_NO"),
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.ConfirmPhoneConnected;
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }
}