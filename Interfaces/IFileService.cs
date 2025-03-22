using Telegram.Bot;

namespace IAndIFamilySupport.API.Interfaces;

public interface IFileService
{
    Task SendPhotoAsync(ITelegramBotClient bot, long chatId, string photoKey, string? businessConnectionId);
    Task SendModelPhotoAsync(ITelegramBotClient bot, long chatId, string model, string? businessConnectionId);

    Task SendConnectionPhotoAsync(ITelegramBotClient bot, long chatId, string deviceType, string deviceModel,
        int step, string? businessConnectionId);

    Task SendPdfInstructionAsync(ITelegramBotClient bot, long chatId, string model, string? businessConnectionId);

    Task SendVideoAsync(ITelegramBotClient bot, long chatId,
        string deviceType,
        string deviceModel, string? businessConnectionId);

    Task SendHelpPhotoAsync(ITelegramBotClient bot, long chatId, string model, string? businessConnectionId);
}