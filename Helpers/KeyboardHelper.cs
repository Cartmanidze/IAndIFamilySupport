using Telegram.Bot.Types.ReplyMarkups;

namespace IAndIFamilySupport.API.Helpers;

public static class KeyboardHelper
{
    public static InlineKeyboardMarkup RecorderModels()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("R8 PLUS (8,32,64)", "RECORDER_R8PLUS"),
                InlineKeyboardButton.WithCallbackData("R3 (8,32,64)", "RECORDER_R3"),
                InlineKeyboardButton.WithCallbackData("R8 (8,32,64)", "RECORDER_R8")
            ],
            [
                InlineKeyboardButton.WithCallbackData("инструкция PDF #1", "PDF_R8PLUS"),
                InlineKeyboardButton.WithCallbackData("инструкция PDF #2", "PDF_R3"),
                InlineKeyboardButton.WithCallbackData("инструкция PDF #3", "PDF_R8")
            ]
        ]);
    }

    public static InlineKeyboardMarkup ProblemMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("Как подключить?", "PROBLEM_CONNECT"),
                InlineKeyboardButton.WithCallbackData("Не воспроизводится запись", "PROBLEM_NOTPLAY"),
                InlineKeyboardButton.WithCallbackData("Помощь в настройке", "PROBLEM_SETTINGS")
            ]
        ]);
    }

    public static InlineKeyboardMarkup DeviceMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("Телефон", "DEVICE_PHONE"),
                InlineKeyboardButton.WithCallbackData("Компьютер", "DEVICE_PC")
            ]
        ]);
    }

    public static InlineKeyboardMarkup PhoneModelMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("OC Windows", "PHONE_WINDOWS"),
                InlineKeyboardButton.WithCallbackData("MacOS", "PHONE_MACOS")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Apple iPhone до 15", "PHONE_IPHONE_OLD"),
                InlineKeyboardButton.WithCallbackData("Apple iPhone после 15", "PHONE_IPHONE_NEW")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Samsung", "PHONE_SAMSUNG"),
                InlineKeyboardButton.WithCallbackData("Honor", "PHONE_HONOR")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Xiaomi", "PHONE_XIAOMI"),
                InlineKeyboardButton.WithCallbackData("Vivo", "PHONE_VIVO")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Huawei", "PHONE_HUAWEI"),
                InlineKeyboardButton.WithCallbackData("POCO", "PHONE_POCO")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Redmi", "PHONE_REDMI"),
                InlineKeyboardButton.WithCallbackData("Realme", "PHONE_REALME")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Tecno", "PHONE_TECNO"),
                InlineKeyboardButton.WithCallbackData("Нет моей модели", "PHONE_NOT_LISTED")
            ]
        ]);
    }

    public static InlineKeyboardMarkup YesNoMenu(string yesData, string noData)
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("Да, получилось", yesData),
                InlineKeyboardButton.WithCallbackData("Нет, не получилось", noData)
            ]
        ]);
    }

    public static InlineKeyboardMarkup FinishMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("Проблема решена", "PROBLEM_SOLVED"),
                InlineKeyboardButton.WithCallbackData("Другая ошибка", "PROBLEM_OTHER")
            ]
        ]);
    }

    // ... и т.д. — Добавляйте нужные клавиатуры для всех ветвей (Настройка MRECSET, гол. активация и прочее).
}