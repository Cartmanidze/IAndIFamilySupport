using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     После вывода инструкции (гос.активация или другие настройки),
///     пользователь отвечает Да/Нет (SETTINGS_OK / SETTINGS_NOT_OK).
/// </summary>
[CallbackRoute("SETTINGS_OK")]
[CallbackRoute("SETTINGS_NOT_OK")]
public record SettingsSolutionResultCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);