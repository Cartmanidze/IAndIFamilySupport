using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь ввёл /start.
/// </summary>
[MessageRoute("/start")]
public record StartCommand(Message Message, CallbackQuery? CallbackQuery) : BaseCommand(Message, CallbackQuery);