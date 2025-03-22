using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands.Base;

public abstract record BaseCommand(Message? Message, CallbackQuery? CallbackQuery) : IRequest<Unit>
{
    public long GetChatId()
    {
        if (Message != null) return Message.Chat.Id;
        if (CallbackQuery?.Message != null) return CallbackQuery.Message.Chat.Id;
        throw new InvalidOperationException("Ни Message, ни CallbackQuery не содержат информацию о чате.");
    }

    public long GetUserId()
    {
        if (CallbackQuery?.Message != null) return CallbackQuery!.From.Id;
        if (Message != null) return Message.From!.Id;
        throw new InvalidOperationException("Ни Message, ни CallbackQuery не содержат информацию о чате.");
    }

    public string? GetBusinessConnectionId()
    {
        if (Message is { From: not null }) return Message.BusinessConnectionId;
        if (CallbackQuery is not null) return CallbackQuery.Message?.BusinessConnectionId;
        throw new InvalidOperationException("Ни Message, ни CallbackQuery не содержат информацию о бизнес соединении.");
    }
}