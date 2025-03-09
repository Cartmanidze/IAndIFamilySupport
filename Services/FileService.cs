using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Services;

internal sealed class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly Dictionary<string, string> _modelPhotoMap;
    private readonly Dictionary<string, string> _pdfInstructionMap;
    
    private readonly Dictionary<string, string> _photoMap;
    private readonly IResourceService _resourceService;

    public FileService(IResourceService resourceService, ILogger<FileService> logger)
    {
        _resourceService = resourceService;
        _logger = logger;

        var allResources = _resourceService.GetAvailableResources().ToArray();
        _logger.LogInformation("Available resources: {ResourceCount}", allResources.Length);
        foreach (var resource in allResources) _logger.LogDebug("Resource: {ResourceName}", resource);
        
        _photoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CONNECTION_PHOTO", "Порядок подключения.jpg" },
            { "FILES_FOLDER", "Файлы.jpg" },
            { "UNTITLED_FOLDER", "Untitled.jpg" },
            { "RECORD_FOLDER", "Record.jpg" }
        };

        _modelPhotoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "R8PLUS", "Фото моделий/R8 PLUS.jpg" },
            { "R3", "Фото моделий/R3.jpg" },
            { "R8", "Фото моделий/R8.jpg" }
        };

        _pdfInstructionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "R8PLUS", "Инструкции pdf/PLUS +NEW.pdf" },
            { "R3", "Инструкции pdf/R3 кулон NEW.pdf" },
            { "R8", "Инструкции pdf/R8 (8,32,64гб) NEW.pdf" }
        };
    }

    public async Task SendPhotoAsync(ITelegramBotClient bot, long chatId, string photoKey, string caption = "")
    {
        if (_photoMap.TryGetValue(photoKey, out var photoPath))
        {
            await SendResourcePhotoAsync(bot, chatId, photoPath, caption);
        }
        else
        {
            _logger.LogWarning("Фото с ключом {PhotoKey} не найдено в маппинге", photoKey);

            await bot.SendMessage(
                chatId,
                "Фото не найдено. " +
                (string.IsNullOrEmpty(caption) ? "" : caption));
        }
    }

    public async Task SendModelPhotoAsync(ITelegramBotClient bot, long chatId, string model, string caption = "")
    {
        if (_modelPhotoMap.TryGetValue(model, out var photoPath))
        {
            await SendResourcePhotoAsync(bot, chatId, photoPath, caption);
        }
        else
        {
            _logger.LogWarning("Фото модели {Model} не найдено в маппинге", model);

            await bot.SendMessage(
                chatId,
                "Фото модели не найдено. " +
                (string.IsNullOrEmpty(caption) ? "" : caption));
        }
    }

    public async Task SendPdfInstructionAsync(ITelegramBotClient bot, long chatId, string model)
    {
        try
        {
            _logger.LogInformation("Attempting to send PDF instruction for model {Model}", model);

            if (!_pdfInstructionMap.TryGetValue(model, out var pdfPath))
            {
                _logger.LogWarning("PDF path not found for model {Model}", model);
                await bot.SendMessage(
                    chatId,
                    $"К сожалению, не удалось найти PDF инструкцию для модели {ModelHelper.GetUserFriendlyModelName(model)}.");
                return;
            }

            var pdfBytes = _resourceService.GetResourceBytes(pdfPath);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                _logger.LogWarning("Failed to load PDF file from path {PdfPath}", pdfPath);
                await bot.SendMessage(
                    chatId,
                    $"К сожалению, не удалось загрузить PDF инструкцию для модели {ModelHelper.GetUserFriendlyModelName(model)}.");
                
                var pdfResources = _resourceService.GetAvailableResources()
                    .Where(r => r.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogInformation("Available PDF resources: {PdfResources}",
                    string.Join(", ", pdfResources));

                return;
            }
            
            var fileName = model switch
            {
                "R8PLUS" => "R8_PLUS_инструкция.pdf",
                "R3" => "R3_инструкция.pdf",
                "R8" => "R8_инструкция.pdf",
                _ => $"{model}_инструкция.pdf"
            };
            
            _logger.LogInformation("Sending PDF instruction for model {Model}, size: {Size} bytes",
                model, pdfBytes.Length);

            using var ms = new MemoryStream(pdfBytes);
            await bot.SendDocument(
                chatId: chatId,
                document: new InputFileStream(ms, fileName),
                caption: $"Инструкция по использованию диктофона {ModelHelper.GetUserFriendlyModelName(model)}");

            _logger.LogInformation("Successfully sent PDF instruction for model {Model}", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending PDF instruction for model {Model}", model);

            await bot.SendMessage(
                chatId,
                "Произошла ошибка при отправке PDF инструкции. Пожалуйста, попробуйте позже или обратитесь к специалисту поддержки.");
        }
    }

    public async Task SendConnectionPhotoAsync(ITelegramBotClient bot, long chatId, string deviceType,
        string deviceModel, int step = 1, string caption = "")
    {
        string folderPath;
        string fileName;

        // Determine the folder path based on device type and model
        if (deviceType == "PHONE")
        {
            folderPath = deviceModel switch
            {
                "IPHONE_OLD" => "Айфон до 15",
                "IPHONE_NEW" => "Айфон после 15",
                "SAMSUNG" => "Samsung",
                "HONOR" => "Honor",
                "XIAOMI" => "Xiaomi",
                "HUAWEI" => "Huawei",
                "POCO" => "POCO",
                "REDMI" => "Redmi",
                "REALME" => "Realme",
                "TECNO" => "Tecno",
                "VIVO" => "Vivo",
                _ => "Samsung"
            };

            fileName = deviceModel switch
            {
                "IPHONE_OLD" or "IPHONE_NEW" => step switch
                {
                    1 => "Порядок подключения.jpg",
                    2 => "Файлы.jpg",
                    3 => "Untitled.jpg",
                    4 => "Record.jpg",
                    _ => "Порядок подключения.jpg"
                },
                "SAMSUNG" => step switch
                {
                    1 => "1.Порядок подключения.jpg",
                    2 => "2.папка с диктофоном.jpg",
                    _ => "1.Порядок подключения.jpg"
                },
                _ => "Порядок подключения.jpg"
            };
        }
        else
        {
            folderPath = deviceModel == "MACOS" ? "Подключение к MAC" : "Подключение к WIN";
            
            fileName = step switch
            {
                1 => "1. Порядок подключения.jpg",
                2 => deviceModel == "MACOS" ? "2. Местонахождение.jpg" : "2. Местоположение.png",
                3 => deviceModel == "MACOS" ? "3. Пака с диктофоном.jpg" : "3. папка с диктофоном.png",
                _ => "1. Порядок подключения.jpg"
            };
        }

        var resourcePath = folderPath + "/" + fileName;
        await SendResourcePhotoAsync(bot, chatId, resourcePath, caption);
    }

    public async Task SendVideoAsync(ITelegramBotClient bot, long chatId, string videoPath, string caption = "")
    {
        var videoBytes = _resourceService.GetResourceBytes(videoPath);

        if (videoBytes != null && videoBytes.Length > 0)
        {
            try
            {
                using var ms = new MemoryStream(videoBytes);
                await bot.SendVideo(
                    chatId: chatId,
                    video: new InputFileStream(ms, Path.GetFileName(videoPath)),
                    caption: caption);

                _logger.LogInformation("Успешно отправлено видео {VideoPath}", videoPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке видео {VideoPath}", videoPath);

                await bot.SendMessage(
                    chatId,
                    "К сожалению, не удалось отправить видео. " +
                    (string.IsNullOrEmpty(caption) ? "" : caption));
            }
        }
        else
        {
            _logger.LogWarning("Видео {VideoPath} не найдено или пустое", videoPath);
            
            var videoResources = _resourceService.GetAvailableResources()
                .Where(r => r.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                            r.EndsWith(".mov", StringComparison.OrdinalIgnoreCase))
                .ToList();

            _logger.LogInformation("Доступные видео ресурсы: {VideoResources}",
                string.Join(", ", videoResources));

            await bot.SendMessage(
                chatId,
                "Видео не найдено. " +
                (string.IsNullOrEmpty(caption) ? "" : caption));
        }
    }

    private async Task SendResourcePhotoAsync(ITelegramBotClient bot, long chatId, string resourcePath, string caption)
    {
        var imageBytes = _resourceService.GetResourceBytes(resourcePath);

        if (imageBytes != null && imageBytes.Length > 0)
        {
            try
            {
                using var ms = new MemoryStream(imageBytes);
                await bot.SendPhoto(
                    chatId: chatId,
                    photo: new InputFileStream(ms, Path.GetFileName(resourcePath)),
                    caption: caption);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке фото {ResourcePath}", resourcePath);

                await bot.SendMessage(
                    chatId,
                    "К сожалению, не удалось отправить фото. " +
                    (string.IsNullOrEmpty(caption) ? "" : caption));
            }
        }
        else
        {
            _logger.LogWarning("Ресурс {ResourcePath} не найден или пустой", resourcePath);
            
            var folder = Path.GetDirectoryName(resourcePath)?.Replace('\\', '/');
            
            await bot.SendMessage(
                chatId,
                "Фото не найдено в ресурсах. " +
                (string.IsNullOrEmpty(caption) ? "" : caption));
        }
    }
}