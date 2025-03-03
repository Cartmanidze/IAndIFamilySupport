namespace IAndIFamilySupport.API.Repositories;

public static class CommonPhoneConnectionRepository
{
    /// <summary>
    ///     Вопрос: удалось ли подключить диктофон к телефону?
    /// </summary>
    public const string DidConnectToPhone = "Получилось подключить диктофон к телефону?";

    /// <summary>
    ///     Ответ: да, получилось
    /// </summary>
    public const string SuccessYes = "Да, получилось";

    /// <summary>
    ///     Ответ: нет, не получилось
    /// </summary>
    public const string SuccessNo = "Нет, не получилось";

    /// <summary>
    ///     Благодарственное сообщение и предложение оставить отзыв на ВБ
    /// </summary>
    public const string ThanksMessage =
        "Спасибо за ваше обращение. Если у вас будут другие вопросы пишите. Так же будем рады вашему отзыву на ВБ";

    /// <summary>
    ///     Текст о переводе на специалиста техподдержки
    /// </summary>
    public const string TransferToSupport = "Переводим вас на специалиста  технической поддержки";
}