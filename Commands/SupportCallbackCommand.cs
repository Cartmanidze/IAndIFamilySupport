using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь нажал любую Inline-кнопку, находясь на шаге TransferToSupport.
/// </summary>
public record SupportCallbackCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);