using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Services;

internal sealed class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly Dictionary<string, string> _modelHelpPhotoMap;
    private readonly Dictionary<string, string> _modelPhotoMap;
    private readonly Dictionary<string, string> _pdfInstructionMap;

    private readonly Dictionary<string, string> _photoMap;
    private readonly IResourceService _resourceService;

    public FileService(IResourceService resourceService, ILogger<FileService> logger)
    {
        _resourceService = resourceService;
        _logger = logger;

        LogAvailableResources();

        // Пример маппинга «ключ → реальный файл»
        _photoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CONNECTION_PHOTO", "Connection.jpg" },
            { "FILES_FOLDER", "Files.jpg" },
            { "UNTITLED_FOLDER", "Untitled.jpg" },
            { "RECORD_FOLDER", "Record.jpg" },
            { "EncodingError", "EncodingError.png" },
            { "EncodingErrorSolution", "EncodingErrorSolution.png" }
        };

        _modelPhotoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "R8PLUS", "Models.R8PLUS.jpg" },
            { "R3", "Models.R3.jpg" },
            { "R8", "Models.R8.jpg" }
        };

        _modelHelpPhotoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "R8PLUS", "Help.R8PLUSR3Mrecsrt.png" },
            { "R3", "Help.R8PLUSR3Mrecsrt.png" },
            { "R8", "Help.R8Mrecsrt.png" }
        };

        _pdfInstructionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "R8PLUS", "Instructions.R8PLUS.pdf" },
            { "R3", "Instruction.R3.pdf" },
            { "R8", "Instructions.R8.pdf" }
        };
    }

    #region Logging

    private void LogAvailableResources()
    {
        var allResources = _resourceService.GetAvailableResources().ToArray();
        _logger.LogInformation("Доступные вшитые ресурсы: {ResourceCount}", allResources.Length);
        foreach (var resource in allResources) _logger.LogDebug("Ресурс: {ResourceName}", resource);
    }

    #endregion

    #region Public Methods

    public async Task SendPhotoAsync(ITelegramBotClient bot, long chatId, string photoKey, string? businessConnectionId)
    {
        if (_photoMap.TryGetValue(photoKey, out var photoPath))
        {
            await SendResourcePhotoAsync(bot, chatId, photoPath, businessConnectionId);
        }
        else
        {
            _logger.LogWarning("Фото с ключом {PhotoKey} не найдено в _photoMap", photoKey);
            await bot.SendMessage(
                chatId,
                "Фото не найдено",
                businessConnectionId: businessConnectionId);
        }
    }

    public async Task SendModelPhotoAsync(ITelegramBotClient bot, long chatId, string model,
        string? businessConnectionId)
    {
        if (_modelPhotoMap.TryGetValue(model, out var photoPath))
        {
            await SendResourcePhotoAsync(bot, chatId, photoPath, businessConnectionId);
        }
        else
        {
            _logger.LogWarning("Фото модели {Model} не найдено в маппинге", model);

            await bot.SendMessage(
                chatId,
                "Фото модели не найдено",
                businessConnectionId: businessConnectionId
            );
        }
    }

    public async Task SendPdfInstructionAsync(ITelegramBotClient bot, long chatId, string model,
        string? businessConnectionId)
    {
        try
        {
            _logger.LogInformation("Пытаемся отправить PDF-инструкцию для модели {Model}", model);

            if (!_pdfInstructionMap.TryGetValue(model, out var pdfPath))
            {
                _logger.LogWarning("PDF-файл для модели {Model} не найден в _pdfInstructionMap", model);
                await bot.SendMessage(
                    chatId,
                    $"К сожалению, не удалось найти PDF инструкцию для модели {ModelHelper.GetUserFriendlyModelName(model)}.",
                    businessConnectionId: businessConnectionId);
                return;
            }

            var pdfBytes = _resourceService.GetResourceBytes(pdfPath);
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                _logger.LogWarning("Не удалось загрузить PDF-файл по пути {PdfPath}", pdfPath);
                await bot.SendMessage(
                    chatId,
                    $"К сожалению, не удалось загрузить PDF-инструкцию для модели {ModelHelper.GetUserFriendlyModelName(model)}.",
                    businessConnectionId: businessConnectionId
                );

                var pdfResources = _resourceService.GetAvailableResources()
                    .Where(r => r.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogInformation("Список доступных PDF-ресурсов: {PdfResources}",
                    string.Join(", ", pdfResources));
                return;
            }

            var fileName = model switch
            {
                "R8PLUS" => "Instructions.R8PLUS.pdf",
                "R3" => "Instruction.R3.pdf",
                "R8" => "Instructions.R8.pdf",
                _ => $"Instructions.{model}.pdf"
            };

            _logger.LogInformation("Отправляем PDF-инструкцию для модели {Model}, размер: {Size} байт", model,
                pdfBytes.Length);

            using var ms = new MemoryStream(pdfBytes);
            await bot.SendDocument(
                chatId,
                new InputFileStream(ms, fileName),
                $"Инструкция к диктофону {ModelHelper.GetUserFriendlyModelName(model)}",
                businessConnectionId: businessConnectionId
            );

            _logger.LogInformation("PDF-инструкция для модели {Model} успешно отправлена", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке PDF-инструкции для модели {Model}", model);
            await bot.SendMessage(
                chatId,
                "Произошла ошибка при отправке PDF. Пожалуйста, попробуйте позже или обратитесь в поддержку.",
                businessConnectionId: businessConnectionId
            );
        }
    }

    public async Task SendConnectionPhotoAsync(
        ITelegramBotClient bot,
        long chatId,
        string deviceType,
        string deviceModel,
        int step,
        string? businessConnectionId)
    {
        // Определяем папку
        var folderPath = ResolveFolderPath(deviceType, deviceModel);
        // Определяем имя файла для текущего шага
        var fileName = ResolveConnectionFileName(deviceType, deviceModel, step);

        var resourcePath = $"{folderPath}.{fileName}";
        _logger.LogInformation(
            "Отправляем фото подключения: deviceType={DeviceType}, deviceModel={DeviceModel}, step={Step}, resourcePath={ResourcePath}",
            deviceType, deviceModel, step, resourcePath
        );

        await SendResourcePhotoAsync(bot, chatId, resourcePath, businessConnectionId);
    }

    public async Task SendVideoAsync(ITelegramBotClient bot, long chatId,
        string deviceType,
        string deviceModel,
        string? businessConnectionId)
    {
        // Определяем папку
        var folderPath = ResolveFolderPath(deviceType, deviceModel);
        const string fileName = "EnableAccessories.mp4";
        var videoPath = $"{folderPath}.{fileName}";
        var videoBytes = _resourceService.GetResourceBytes(videoPath);
        if (videoBytes != null && videoBytes.Length > 0)
        {
            try
            {
                using var ms = new MemoryStream(videoBytes);
                await bot.SendVideo(chatId, new InputFileStream(ms, fileName),
                    businessConnectionId: businessConnectionId);
                _logger.LogInformation("Видео {VideoPath} успешно отправлено", videoPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке видео {VideoPath}", videoPath);
                await bot.SendMessage(
                    chatId,
                    "К сожалению, не удалось отправить видео",
                    businessConnectionId: businessConnectionId
                );
            }
        }
        else
        {
            _logger.LogWarning("Видео {VideoPath} не найдено или пустое", videoPath);
            var videoResources = _resourceService.GetAvailableResources()
                .Where(r => r.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                            r.EndsWith(".mov", StringComparison.OrdinalIgnoreCase))
                .ToList();

            _logger.LogInformation("Доступные видео-ресурсы: {VideoResources}", string.Join(", ", videoResources));
            await bot.SendMessage(
                chatId,
                "Видео не найдено.",
                businessConnectionId: businessConnectionId
            );
        }
    }

    public async Task SendHelpPhotoAsync(ITelegramBotClient bot, long chatId, string model,
        string? businessConnectionId)
    {
        if (_modelHelpPhotoMap.TryGetValue(model, out var photoPath))
        {
            await SendResourcePhotoAsync(bot, chatId, photoPath, businessConnectionId);
        }
        else
        {
            _logger.LogWarning("Фото модели {Model} не найдено в маппинге", model);

            await bot.SendMessage(
                chatId,
                "Фото модели не найдено.",
                businessConnectionId: businessConnectionId);
        }
    }

    #endregion

    #region Private Helpers

    private async Task SendResourcePhotoAsync(
        ITelegramBotClient bot,
        long chatId,
        string resourcePath,
        string? businessConnectionId)
    {
        var imageBytes = _resourceService.GetResourceBytes(resourcePath);
        if (imageBytes is not null && imageBytes.Length > 0)
        {
            try
            {
                using var ms = new MemoryStream(imageBytes);
                var fileName = Path.GetFileName(resourcePath);

                await bot.SendPhoto(chatId, new InputFileStream(ms, fileName),
                    businessConnectionId: businessConnectionId);
                _logger.LogInformation("Фото {ResourcePath} успешно отправлено", resourcePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке фото {ResourcePath}", resourcePath);
                await bot.SendMessage(
                    chatId,
                    "Не удалось отправить фото",
                    businessConnectionId: businessConnectionId
                );
            }
        }
        else
        {
            _logger.LogWarning("Ресурс {ResourcePath} не найден или пустой", resourcePath);
            await bot.SendMessage(
                chatId,
                "Фото не найдено в ресурсах",
                businessConnectionId: businessConnectionId
            );
        }
    }

    private string ResolveFolderPath(string deviceType, string deviceModel)
    {
        // Переводим в верхний регистр для надёжного сравнения
        if (deviceType.Equals("PHONE", StringComparison.OrdinalIgnoreCase))
            return deviceModel.ToUpperInvariant() switch
            {
                "IPHONE_OLD" => "IPhoneBefore15",
                "IPHONE_NEW" => "IPhoneAfter15",
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

        // Если deviceType != PHONE, считаем, что это компьютер (Mac / Win)
        return deviceModel.Equals("MACOS", StringComparison.OrdinalIgnoreCase)
            ? "Mac"
            : "Win";
    }

    private string ResolveConnectionFileName(string deviceType, string deviceModel, int step)
    {
        if (deviceType.Equals("PHONE", StringComparison.OrdinalIgnoreCase))
            return deviceModel.ToUpperInvariant() switch
            {
                "IPHONE_NEW" => step switch
                {
                    1 => "Connection.jpg",
                    2 => "Files.jpg",
                    3 => "Untitled.jpg",
                    4 => "Record.jpg",
                    _ => "Connection.jpg"
                },
                "IPHONE_OLD" => step switch
                {
                    1 => "OTG.jpg",
                    2 => "Connection.jpg",
                    3 => "Files.jpg",
                    4 => "Untitled.jpg",
                    5 => "Record.jpg",
                    _ => "Connection.jpg"
                },
                _ => step switch
                {
                    1 => "Connection.jpg",
                    2 => "NotificationShade.jpg",
                    _ => "Connection.jpg"
                }
            };

        // Mac или Win
        var isMac = deviceModel.Equals("MACOS", StringComparison.OrdinalIgnoreCase);
        return step switch
        {
            1 => "Connection.jpg",
            2 => isMac ? "Location.jpg" : "Location.png",
            3 => isMac ? "Folder.jpg" : "Folder.png",
            _ => "Connection.jpg"
        };
    }

    #endregion
}