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
        ScenarioStep.HowToConnectPcModel,
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

            case ScenarioStep.HowToConnectPcModel:
                await HandlePcModelSelection(bot, chatId, data, state);
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

        switch (deviceType)
        {
            case "PHONE":
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.SpecifyPhoneModel,
                    replyMarkup: KeyboardHelper.PhoneModelMenu()
                );

                state.CurrentStep = ScenarioStep.HowToConnectPhoneModel;
                break;
            case "PC":
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.SpecifyPhoneModel,
                    replyMarkup: KeyboardHelper.PcModelMenu()
                );

                state.CurrentStep = ScenarioStep.HowToConnectPcModel;
                break;
            default:
                await bot.SendMessage(
                    chatId,
                    "Неизвестное устройство. Пожалуйста, выберите из предложенных вариантов."
                );
                return;
        }

        _stateService.UpdateUserState(state);
    }

    private async Task HandlePcModelSelection(ITelegramBotClient bot, long chatId, string data,
        TelegramUserState state)
    {
        var pcModel = data.StartsWith("PC_") ? data[3..] : data;
        await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel);
        await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 2);
        await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", pcModel, 3);

        await bot.SendMessage(
            chatId,
            ConnectionScenarioTextRepository.DidConnectToComputer,
            replyMarkup: KeyboardHelper.YesNoMenu("PC_CONNECTED_YES", "PC_CONNECTED_NO")
        );

        state.CurrentStep = ScenarioStep.ConfirmPcConnected;
    }

    private async Task HandlePhoneModelSelection(ITelegramBotClient bot, long chatId, string data,
        TelegramUserState state)
    {
        var phoneModel = data.StartsWith("PHONE_") ? data[6..] : data;
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
        else
        {
            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", phoneModel);

            if (state.SelectedPhoneModel is "IPHONE_OLD")
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.NeededOtg
                );

            if (state.SelectedPhoneModel is "IPHONE_OLD" or "IPHONE_NEW")
            {
                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 2);
                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 3);
                await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 4);

                if (state.SelectedPhoneModel is "IPHONE_OLD")
                    await _fileService.SendConnectionPhotoAsync(bot, chatId, "PHONE", state.SelectedPhoneModel, 5);

                await bot.SendMessage(
                    chatId,
                    AccessoriesCheckRepository.CheckAccessories
                );
            }
            else
            {
                await bot.SendMessage(
                    chatId,
                    ConnectionScenarioTextRepository.OtgActivationRecommendation
                );

                await bot.SendMessage(chatId, ConnectionScenarioTextRepository.GetOtgActivationInstruction(phoneModel));
            }

            await bot.SendMessage(chatId, ConnectionScenarioTextRepository.DictaphoneFolderLocation);

            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.DidConnectToPhone,
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
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.ThanksMessage
            );

            state.CurrentStep = ScenarioStep.Finish;
        }
        else if (data == "PHONE_CONNECTED_NO")
        {
            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await bot.SendMessage(
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
            await bot.SendMessage(
                chatId,
                CommonConnectionRepository.ThanksMessage
            );

            state.CurrentStep = ScenarioStep.Finish;
        }
        else if (data == "PC_CONNECTED_NO")
        {
            var deviceModel = state.SelectedPhoneModel == "MACOS" ? "MACOS" : "WINDOWS";

            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", deviceModel, 2);
            await _fileService.SendConnectionPhotoAsync(bot, chatId, "PC", deviceModel, 3);

            await bot.SendMessage(
                chatId,
                ConnectionScenarioTextRepository.TransferToSupport
            );

            state.CurrentStep = ScenarioStep.TransferToSupport;
        }
        else
        {
            await bot.SendMessage(
                chatId,
                "Неизвестный ответ. Пожалуйста, выберите из предложенных вариантов."
            );
            return;
        }

        _stateService.UpdateUserState(state);
    }
}