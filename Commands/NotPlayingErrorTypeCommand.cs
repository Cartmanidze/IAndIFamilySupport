using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь на шаге NotPlayingErrorType выбирает "ERROR_CODING" или "ERROR_OTHER".
/// </summary>
[CallbackRoute("ERROR_CODING")]
[CallbackRoute("ERROR_OTHER")]
public record NotPlayingErrorTypeCommand(Update Update) : IRequest<Unit>;