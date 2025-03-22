using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь нажал "PDF_{model}"
/// </summary>
[CallbackRoutePattern(@"^PDF_(?<model>.+)$")]
public record ShowPdfCommand(Message Message, CallbackQuery? CallbackQuery, string Model)
    : BaseCommand(Message, CallbackQuery);