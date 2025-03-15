using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

[CallbackRoute("PC_CONNECTED_YES")]
[CallbackRoute("PC_CONNECTED_NO")]
public record ConfirmPcConnectionCommand(Update Update) : IRequest<Unit>;