using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал конкретную модель телефона (например PHONE_SAMSUNG, PHONE_XIAOMI и т.д.)
/// </summary>
[CallbackRoutePattern("^PHONE_.+$")]
public record SelectPhoneModelCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);