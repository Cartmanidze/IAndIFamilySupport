using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Strategies;

public class ConnectionScenarioStrategy : IScenarioStrategy
{
    private readonly IFileService _fileService;
    private readonly ILogger<ConnectionScenarioStrategy> _logger;
    private readonly IStateService _stateService;

    public ConnectionScenarioStrategy(
        IStateService stateService,
        IFileService fileService,
        ILogger<ConnectionScenarioStrategy> logger)
    {
        _stateService = stateService;
        _fileService = fileService;
        _logger = logger;
    }

    public IEnumerable<ScenarioStep> TargetSteps =>
    [
        ScenarioStep.HowToConnectDevice,
        ScenarioStep.HowToConnectPhoneModel,
        ScenarioStep.ConfirmPhoneConnected,
        ScenarioStep.ConfirmPcConnected
    ];

    public async Task HandleMessageAsync(ITelegramBotClient bot, Message message, TelegramUserState state)
    {
        if (message.Text == "/start")
        {
            state.CurrentStep = ScenarioStep.Start;
            _stateService.UpdateUserState(state);
            return;
        }

        var chatId = message.Chat.Id;

        await bot.SendMessage(
            chatId,
            "Пожалуйста, выберите действие из меню или напишите /start для начала заново."
        );
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state)
    {
        var chatId = callback.Message!.Chat.Id;
        var data = callback.Data!;

        await bot.AnswerCallbackQuery(callback.Id);

        switch (state.CurrentStep)
        {
            case ScenarioStep.HowToConnectDevice:
                await HandleDeviceSelection(bot, chatId, data, state);
                break;

            case ScenarioStep.HowToConnectPhoneModel:
                await HandlePhoneModelSelection(bot, chatId, data, state);
                break;

            case ScenarioStep.ConfirmPhoneConnected:
                await HandlePhoneConnectionResult(bot, chatId, data, state);
                break;

            case ScenarioStep.ConfirmPcConnected:
                await HandlePcConnectionResult(bot, chatId, data, state);
                break;

            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестный шаг сценария. Пожалуйста, напишите /start для начала заново."
                );
                break;
        }
    }

    private async Task HandleDeviceSelection(ITelegramBotClient bot, long chatId, string data, TelegramUserState state)
    {
        var deviceType = data.Replace("DEVICE_", "");
        state.SelectedDevice = deviceType;

        _logger.LogInformation("User selected device: {DeviceType}", deviceType);

        if (deviceType == "PHONE")
        {
            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.SpecifyPhoneModel,
                replyMarkup: KeyboardHelper.PhoneModelMenu()
            );

            state.CurrentStep = ScenarioStep.HowToConnectPhoneModel;
        }
        else if (deviceType == "PC")
        {
            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", "WINDOWS");

            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.DidConnectToComputer,
                replyMarkup: KeyboardHelper.YesNoMenu("PC_CONNECTED_YES", "PC_CONNECTED_NO")
            );

            state.CurrentStep = ScenarioStep.ConfirmPcConnected;
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Неизвестное устройство. Пожалуйста, выберите из предложенных вариантов."
            );
            return;
        }

        _stateService.UpdateUserState(state);
    }

    private async Task HandlePhoneModelSelection(ITelegramBotClient bot, long chatId, string data,
        TelegramUserState state)
    {
        var phoneModel = data.Replace("PHONE_", "");
        state.SelectedPhoneModel = phoneModel;

        _logger.LogInformation("User selected phone model: {PhoneModel}", phoneModel);

        if (phoneModel == "NOT_LISTED")
        {
            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else if (phoneModel is "WINDOWS" or "MACOS")
        {
            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", phoneModel);

            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.DidConnectToComputer,
                replyMarkup: KeyboardHelper.YesNoMenu("PC_CONNECTED_YES", "PC_CONNECTED_NO")
            );

            state.CurrentStep = ScenarioStep.ConfirmPcConnected;
        }
        else
        {
            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel);

            if (phoneModel is "XIAOMI" or "REDMI" or "POCO")
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.OtgActivationRecommendation
                );

            await bot.SendMessage(
                chatId,
                CommonPhoneConnectionRepository.DidConnectToPhone,
                replyMarkup: KeyboardHelper.YesNoMenu("PHONE_CONNECTED_YES", "PHONE_CONNECTED_NO")
            );

            state.CurrentStep = ScenarioStep.ConfirmPhoneConnected;
        }

        _stateService.UpdateUserState(state);
    }

    private async Task HandlePhoneConnectionResult(ITelegramBotClient bot, long chatId, string data,
        TelegramUserState state)
    {
        if (data == "PHONE_CONNECTED_YES")
        {
            // Show success message
            await bot.SendTextMessageAsync(
                chatId,
                CommonPhoneConnectionRepository.ThanksMessage
            );

            // Update state to finish
            state.CurrentStep = ScenarioStep.Finish;
        }
        else if (data == "PHONE_CONNECTED_NO")
        {
            // Send additional photo(s) based on phone model
            if (state.SelectedPhoneModel == "IPHONE_OLD" || state.SelectedPhoneModel == "IPHONE_NEW")
            {
                // For iPhone: Send more detailed instructions
                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 2);
                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 3);
                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 4);
            }
            else
            {
                // For Android: Check accessories
                await bot.SendTextMessageAsync(
                    chatId,
                    AccessoriesCheckRepository.CheckAccessories
                );

                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 2);
            }

            // Transfer to support
            await bot.SendTextMessageAsync(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            // Update state
            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await bot.SendTextMessageAsync(
                chatId,
                "Неизвестный ответ. Пожалуйста, выберите из предложенных вариантов."
            );
            return;
        }

        _stateService.UpdateUserState(state);
    }

    private async Task HandlePcConnectionResult(ITelegramBotClient bot, long chatId, string data,
        TelegramUserState state)
    {
        if (data == "PC_CONNECTED_YES")
        {
            // Show success message
            await bot.SendTextMessageAsync(
                chatId,
                CommonPhoneConnectionRepository.ThanksMessage
            );

            // Update state to finish
            state.CurrentStep = ScenarioStep.Finish;
        }
        else if (data == "PC_CONNECTED_NO")
        {
            // Send additional instructions
            var deviceModel = state.SelectedPhoneModel == "MACOS" ? "MACOS" : "WINDOWS";

            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", deviceModel, 2);
            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", deviceModel, 3);

            // Transfer to support
            await bot.SendTextMessageAsync(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            // Update state
            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await bot.SendTextMessageAsync(
                chatId,
                "Неизвестный ответ. Пожалуйста, выберите из предложенных вариантов."
            );
            return;
        }

        _stateService.UpdateUserState(state);
    }
}