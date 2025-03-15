using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал "Помощь в настройке" (текст) или "PROBLEM_SETTINGS" (callback).
/// </summary>
[MessageRoute("Помощь в настройке")]
[CallbackRoute("PROBLEM_SETTINGS")]
public record SelectProblemSettingsCommand(Update Update) : IRequest<Unit>;