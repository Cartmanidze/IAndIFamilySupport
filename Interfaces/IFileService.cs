using Telegram.Bot;

namespace IAndIFamilySupport.API.Interfaces;

public interface IFileService
{
    Task SendPhotoAsync(ITelegramBotClient bot, long chatId, string photoKey, string caption = "");
    Task SendModelPhotoAsync(ITelegramBotClient bot, long chatId, string model, string caption = "");

    Task SendConnectionPhotoAsync(ITelegramBotClient bot, long chatId, string deviceType, string deviceModel,
        int step = 1, string caption = "");

    Task SendPdfInstructionAsync(ITelegramBotClient bot, long chatId, string model);

    Task SendVideoAsync(ITelegramBotClient bot, long chatId,
        string deviceType,
        string deviceModel, string caption = "");

    Task SendHelpPhotoAsync(ITelegramBotClient bot, long chatId, string model, string caption = "");
}