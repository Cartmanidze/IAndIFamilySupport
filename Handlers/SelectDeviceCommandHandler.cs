using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

/// <summary>
///     Обрабатывает выбор устройства: DEVICE_PHONE или DEVICE_PC.
///     Сценарный шаг: HowToConnectDevice.
/// </summary>
public class SelectDeviceCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    ILogger<SelectDeviceCommandHandler> logger)
    : IRequestHandler<SelectDeviceCommand, Unit>
{
    public async Task<Unit> Handle(SelectDeviceCommand request, CancellationToken cancellationToken)
    {
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        // Ответим на коллбек, чтобы убрать "крутилку"
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        // Проверяем, что шаг пользователя действительно HowToConnectDevice
        var state = stateService.GetUserState(userId)!;
        var data = callback.Data; // "DEVICE_PHONE" или "DEVICE_PC"
        var deviceType = data!.Replace("DEVICE_", "");
        state.SelectedDevice = deviceType;

        logger.LogInformation("Пользователь {UserId} выбрал устройство: {DeviceType}", userId, deviceType);

        switch (deviceType.ToUpperInvariant())
        {
            case "PHONE":
                // Показать клавиатуру выбора модели телефона
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.SpecifyPhoneModel,
                    replyMarkup: KeyboardHelper.PhoneModelMenu(),
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );
                state.CurrentStep = ScenarioStep.HowToConnectPhoneModel;
                break;

            case "PC":
                // Показать клавиатуру выбора модели ПК
                await bot.SendMessage(chatId,
                    "Перед подключением диктофона к компьютеру, переведите \"бегунок\"- кнопку на диктофоне в положение OFF",
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);
                
                await bot.SendMessage(
                    chatId,
                    "Уточните операционную систему вашего компьютера",
                    replyMarkup: KeyboardHelper.PcModelMenu(),
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );
                state.CurrentStep = ScenarioStep.HowToConnectPcModel;
                break;

            default:
                // Неизвестное устройство
                await bot.SendMessage(
                    chatId,
                    "Неизвестное устройство. Пожалуйста, выберите из предложенных вариантов.",
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );
                break;
        }

        stateService.UpdateUserState(state);

        return Unit.Value;
    }
}