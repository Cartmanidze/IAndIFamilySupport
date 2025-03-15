using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     После вывода инструкции (гос.активация или другие настройки),
///     пользователь отвечает Да/Нет (SETTINGS_OK / SETTINGS_NOT_OK).
/// </summary>
[CallbackRoute("SETTINGS_OK")]
[CallbackRoute("SETTINGS_NOT_OK")]
public record SettingsSolutionResultCommand(Update Update) : IRequest<Unit>;