using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IAndIFamilySupport.API.Extensions;

internal static class UpdateExtensions
{
    internal static User? ExtractUser(this Update update)
    {
        return update switch
        {
            { Type: UpdateType.CallbackQuery, CallbackQuery: not null } =>
                update.CallbackQuery.From,
            { Type: UpdateType.Message, Message: not null } => update.Message.From,
            { Type: UpdateType.BusinessMessage, BusinessMessage: not null } => update.BusinessMessage.From,
            _ => null
        };
    }
}