using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь на шаге NotPlayingSolutionResult выбирает "PROBLEM_SOLVED" или "PROBLEM_OTHER".
/// </summary>
[CallbackRoute("PROBLEM_SOLVED")]
[CallbackRoute("PROBLEM_OTHER")]
public record NotPlayingSolutionResultCommand(Update Update) : IRequest<Unit>;