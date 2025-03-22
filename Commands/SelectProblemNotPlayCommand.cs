using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал "Не воспроизводится запись?" или callback "PROBLEM_NOTPLAY".
/// </summary>
[MessageRoute("Не воспроизводится запись")]
[CallbackRoute("PROBLEM_NOTPLAY")]
public record SelectProblemNotPlayCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);