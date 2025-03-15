using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбирает устройство: DEVICE_PHONE / DEVICE_PC
/// </summary>
[CallbackRoute("DEVICE_PHONE")]
[CallbackRoute("DEVICE_PC")]
public record SelectDeviceCommand(Update Update) : IRequest<Unit>;