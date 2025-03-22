using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал "Помощь в настройке" (текст) или "PROBLEM_SETTINGS" (callback).
/// </summary>
[MessageRoute("Помощь в настройке")]
[CallbackRoute("PROBLEM_SETTINGS")]
public record SelectProblemSettingsCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);