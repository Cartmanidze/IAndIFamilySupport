using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

[CallbackRoute("PC_CONNECTED_YES")]
[CallbackRoute("PC_CONNECTED_NO")]
public record ConfirmPcConnectionCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);