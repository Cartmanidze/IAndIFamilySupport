using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь на шаге NotPlayingErrorType выбирает "ERROR_CODING" или "ERROR_OTHER".
/// </summary>
[CallbackRoute("ERROR_CODING")]
[CallbackRoute("ERROR_OTHER")]
public record NotPlayingErrorTypeCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);