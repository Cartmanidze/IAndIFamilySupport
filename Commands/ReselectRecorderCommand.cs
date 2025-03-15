using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

[CallbackRoute("RESELECT")]
public record ReselectRecorderCommand(Update Update) : IRequest<Unit>;