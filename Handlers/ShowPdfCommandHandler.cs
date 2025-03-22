using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Interfaces;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class ShowPdfCommandHandler(
    ITelegramBotClient bot,
    IFileService fileService) : IRequestHandler<ShowPdfCommand, Unit>
{
    public async Task<Unit> Handle(ShowPdfCommand request, CancellationToken cancellationToken)
    {
        var callback = request.CallbackQuery!;
        var chatId = request.GetChatId();
        var businessConnectionId = request.GetBusinessConnectionId();

        await fileService.SendPdfInstructionAsync(bot, chatId, request.Model, businessConnectionId);

        await bot.SendMessage(chatId, "Инструкция отправлена!", businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}