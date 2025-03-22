using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

[CallbackRoute("PHONE_CONNECTED_YES")]
[CallbackRoute("PHONE_CONNECTED_NO")]
public record ConfirmPhoneConnectionCommand(Message Message, CallbackQuery? CallbackQuery)
    : BaseCommand(Message, CallbackQuery);