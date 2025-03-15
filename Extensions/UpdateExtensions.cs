using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IAndIFamilySupport.API.Extensions;

internal static class UpdateExtensions
{
    internal static (long chatId, long userId) ExtractChatAndUserId(this Update update)
    {
        return update switch
        {
            { Type: UpdateType.CallbackQuery, CallbackQuery: not null } => (update.CallbackQuery.Message!.Chat.Id,
                update.CallbackQuery.From.Id),
            { Type: UpdateType.Message, Message: not null } => (update.Message.Chat.Id, update.Message.From?.Id ?? 0),
            _ => (0, 0)
        };
    }
}