using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь нажал "CONFIRM_RECORDER_{model}"
/// </summary>
[CallbackRoutePattern(@"^CONFIRM_RECORDER_(?<model>.+)$")]
public record ConfirmRecorderCommand(Message Message, CallbackQuery? CallbackQuery, string Model)
    : BaseCommand(Message, CallbackQuery);