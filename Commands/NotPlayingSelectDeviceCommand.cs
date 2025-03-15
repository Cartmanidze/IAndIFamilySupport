using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь на шаге NotPlayingMenu выбирает "PLAYBACK_PHONE" или "PLAYBACK_PC".
/// </summary>
[CallbackRoute("PLAYBACK_PHONE")]
[CallbackRoute("PLAYBACK_PC")]
public record NotPlayingSelectDeviceCommand(Update Update) : IRequest<Unit>;