using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал "Как подключить?" (текст) или нажал callback "PROBLEM_CONNECT".
/// </summary>
[MessageRoute("Как подключить?")] // текстовое сообщение
[MessageRoute("/как подключить")] // если хотите ещё вариант
[CallbackRoute("PROBLEM_CONNECT")] // callback data
public record SelectProblemConnectCommand(Update Update) : IRequest<Unit>;