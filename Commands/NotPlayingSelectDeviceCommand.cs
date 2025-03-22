using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь на шаге NotPlayingMenu выбирает "PLAYBACK_PHONE" или "PLAYBACK_PC".
/// </summary>
[CallbackRoute("PLAYBACK_PHONE")]
[CallbackRoute("PLAYBACK_PC")]
public record NotPlayingSelectDeviceCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);