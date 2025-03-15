using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь нажал inline-кнопку, где callback data вида "RECORDER_XXX".
///     RegEx: "^RECORDER_(?.+)$" означает, что после "RECORDER_" идут любые символы,
///     мы их кладём в группу "model".
/// </summary>
[CallbackRoutePattern(@"^RECORDER_(?<model>.+)$")]
public record SelectRecorderCommand(Update Update, string Model) : IRequest<Unit>;