using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь нажал любую Inline-кнопку, находясь на шаге TransferToSupport.
/// </summary>
public record SupportCallbackCommand(Update Update) : IRequest<Unit>;