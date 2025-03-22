using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь выбирает, что настроить (голосовую активацию или другие параметры).
/// </summary>
[CallbackRoute("VOICE_ACTIVATION")]
[CallbackRoute("OTHER_SETTINGS")]
public record SelectSettingsOptionCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);