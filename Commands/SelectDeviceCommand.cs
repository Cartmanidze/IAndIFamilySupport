using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбирает устройство: DEVICE_PHONE / DEVICE_PC
/// </summary>
[CallbackRoute("DEVICE_PHONE")]
[CallbackRoute("DEVICE_PC")]
public record SelectDeviceCommand(Message Message, CallbackQuery? CallbackQuery) : BaseCommand(Message, CallbackQuery);