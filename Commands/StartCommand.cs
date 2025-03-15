using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь ввёл /start.
/// </summary>
[MessageRoute("/start")]
public record StartCommand(Update Update) : IRequest<Unit>;