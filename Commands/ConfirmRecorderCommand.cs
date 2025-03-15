using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь нажал "CONFIRM_RECORDER_{model}"
/// </summary>
[CallbackRoutePattern(@"^CONFIRM_RECORDER_(?<model>.+)$")]
public record ConfirmRecorderCommand(Update Update, string Model) : IRequest<Unit>;