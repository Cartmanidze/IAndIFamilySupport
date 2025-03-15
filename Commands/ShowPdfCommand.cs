using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь нажал "PDF_{model}"
/// </summary>
[CallbackRoutePattern(@"^PDF_(?<model>.+)$")]
public record ShowPdfCommand(Update Update, string Model) : IRequest<Unit>;