using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал модель ПК (например PC_WINDOWS, PC_MACOS)
/// </summary>
[CallbackRoutePattern(@"^PC_.+$")]
public record SelectPcModelCommand(Update Update) : IRequest<Unit>;