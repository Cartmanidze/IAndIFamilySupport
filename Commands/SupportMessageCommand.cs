using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь отправил текст/сообщение, находясь в TransferToSupport.
/// </summary>
public record SupportMessageCommand(Update Update) : IRequest<Unit>;