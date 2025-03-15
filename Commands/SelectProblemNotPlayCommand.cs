using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал "Не воспроизводится запись?" или callback "PROBLEM_NOTPLAY".
/// </summary>
[MessageRoute("Не воспроизводится запись")]
[CallbackRoute("PROBLEM_NOTPLAY")]
public record SelectProblemNotPlayCommand(Update Update) : IRequest<Unit>;