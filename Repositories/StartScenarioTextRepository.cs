namespace IAndIFamilySupport.API.Repositories;

public static class StartScenarioTextRepository
{
    /// <summary>
    ///     Ответ на первое сообщение
    /// </summary>
    public const string FirstMessageReply = @"
Здравствуйте! 
Добро пожаловать в службу технической поддержки I and I family electronics! Мы рады, что вы обратились к нам. 
Нашей целью является предоставление вам высококачественного обслуживания и оперативное решение ваших вопросов.

Также приглашаем вас подписаться на наш Telegram-канал (https://t.me/IandIfamily). 
Там вы найдете полезные видеообзоры наших продуктов и новинок.

Пожалуйста, опишите вашу проблему или запрос как можно подробнее, и мы сделаем все возможное, чтобы помочь вам в кратчайшие сроки.

Спасибо за ваше доверие и выбор нашего бренда!

С уважением, 
Команда I and I family electronics
";

    /// <summary>
    ///     Дополнительная ссылка на канал, если нужна отдельно
    /// </summary>
    public const string TelegramChannelLink = "https://t.me/IandIfamily, https://t.me/IandIfamily</a>";

    /// <summary>
    ///     Предложение выбрать модель диктофона
    /// </summary>
    public const string ChooseModelPrompt = "Выберите модель диктофона, которую вы приобрели:";

    /// <summary>
    ///     Варианты моделей
    /// </summary>
    public const string R8Plus = "R8 PLUS (8,32,64)";

    public const string R3 = "R3 (8,32,64)";
    public const string R8 = "R8 (8,32,64)";

    /// <summary>
    ///     Ссылки на PDF-инструкции (здесь три одинаковые, при необходимости можно заменить содержимое)
    /// </summary>
    public const string PdfInstruction1 = "инструкция PDF";

    public const string PdfInstruction2 = "инструкция PDF";
    public const string PdfInstruction3 = "инструкция PDF";

    /// <summary>
    ///     Предложение выбрать проблему
    /// </summary>
    public const string ChooseProblemPrompt = "Выберите вопрос, которую необходимо решить:";

    /// <summary>
    ///     Варианты проблем
    /// </summary>
    public const string ProblemHowToConnect = "Как подключить?";

    public const string ProblemNotPlaying = "Не воспроизводится запись";
    public const string ProblemHelpSetup = "Помощь в настройке";
}