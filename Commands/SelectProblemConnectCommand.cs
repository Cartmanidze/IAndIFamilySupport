using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал "Как подключить?" (текст) или нажал callback "PROBLEM_CONNECT".
/// </summary>
[MessageRoute("Как подключить?")] // текстовое сообщение
[MessageRoute("/как подключить")] // если хотите ещё вариант
[CallbackRoute("PROBLEM_CONNECT")] // callback data
public record SelectProblemConnectCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);