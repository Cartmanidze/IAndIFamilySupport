using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал модель ПК (например PC_WINDOWS, PC_MACOS)
/// </summary>
[CallbackRoutePattern("^PC_.+$")]
public record SelectPcModelCommand(Message Message, CallbackQuery? CallbackQuery) : BaseCommand(Message, CallbackQuery);