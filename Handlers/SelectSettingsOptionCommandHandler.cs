using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class SelectSettingsOptionCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService,
    IFileService fileService) : IRequestHandler<SelectSettingsOptionCommand, Unit>
{
    public async Task<Unit> Handle(SelectSettingsOptionCommand request, CancellationToken cancellationToken)
    {
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var userId = request.GetUserId();
        var businessConnectionId = request.GetBusinessConnectionId();

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

        var state = stateService.GetUserState(userId);

        switch (callback.Data)
        {
            case "VOICE_ACTIVATION":
            {
                // Переходим на VoiceActivation
                state.CurrentStep = ScenarioStep.VoiceActivation;

                await bot.SendMessage(chatId, SettingsHelpOptionsRepository.VoiceActivationText,
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken);

                // Спрашиваем: получилось?
                await bot.SendMessage(
                    chatId,
                    "Получилось настроить?",
                    replyMarkup: KeyboardHelper.YesNoMenu("SETTINGS_OK", "SETTINGS_NOT_OK"),
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );

                break;
            }

            case "OTHER_SETTINGS":
            {
                // Переходим на OtherSettings
                state.CurrentStep = ScenarioStep.OtherSettings;

                // Высылаем инструкцию (BIT/GAIN/PART/TIME) для state.SelectedRecorderModel
                await SendOtherSettingsInstructions(userId, chatId, businessConnectionId, cancellationToken);

                // Спросим: "Получилось?"
                await bot.SendMessage(
                    chatId,
                    "Получилось настроить?",
                    replyMarkup: KeyboardHelper.YesNoMenu("SETTINGS_OK", "SETTINGS_NOT_OK"),
                    businessConnectionId: businessConnectionId,
                    cancellationToken: cancellationToken
                );

                break;
            }
        }

        stateService.UpdateUserState(state);
        return Unit.Value;
    }


    private async Task SendOtherSettingsInstructions(long userId, long chatId, string? businessConnectionId,
        CancellationToken ct)
    {
        var state = stateService.GetUserState(userId);
        var model = state.SelectedRecorderModel!; // "R8", "R8PLUS", "R3"
        var otherSettings = SettingsHelpOptionsRepository.OtherSettings[model];
        await fileService.SendHelpPhotoAsync(bot, chatId, model, businessConnectionId);
        await bot.SendMessage(chatId, otherSettings, businessConnectionId: businessConnectionId, cancellationToken: ct);
    }
}