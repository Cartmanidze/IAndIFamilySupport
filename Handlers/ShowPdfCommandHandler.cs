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
        var callback = request.Update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;

        // request.Model = "R8PLUS" и т.п.
        await fileService.SendPdfInstructionAsync(bot, chatId, request.Model);

        // Можно отправить дополнительное сообщение, если хотите
        await bot.SendMessage(chatId, "Инструкция отправлена!", cancellationToken: cancellationToken);

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}