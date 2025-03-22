using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь на шаге NotPlayingSolutionResult выбирает "PROBLEM_SOLVED" или "PROBLEM_OTHER".
/// </summary>
[CallbackRoute("PROBLEM_SOLVED")]
[CallbackRoute("PROBLEM_OTHER")]
public record NotPlayingSolutionResultCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);