using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

[CallbackRoute("PHONE_CONNECTED_YES")]
[CallbackRoute("PHONE_CONNECTED_NO")]
public record ConfirmPhoneConnectionCommand(Update Update) : IRequest<Unit>;