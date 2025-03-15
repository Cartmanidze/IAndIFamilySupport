using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь выбрал конкретную модель телефона (например PHONE_SAMSUNG, PHONE_XIAOMI и т.д.)
/// </summary>
[CallbackRoutePattern(@"^PHONE_.+$")]
public record SelectPhoneModelCommand(Update Update) : IRequest<Unit>;