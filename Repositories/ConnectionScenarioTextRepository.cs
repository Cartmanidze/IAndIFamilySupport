namespace IAndIFamilySupport.API.Repositories;

public static class ConnectionScenarioTextRepository
{
    /// <summary>
    ///     Вопрос о том, как подключить диктофон
    /// </summary>
    public const string HowToConnect = "Как подключить?";

    /// <summary>
    ///     Уточнение, к какому устройству пользователь пытается подключить диктофон
    /// </summary>
    public const string WhichDeviceToConnect = "К какому устройству пытаетесь подключить?";

    /// <summary>
    ///     Вариант ответа: телефон
    /// </summary>
    public const string DevicePhone = "Телефон";

    /// <summary>
    ///     Вариант ответа: компьютер
    /// </summary>
    public const string DeviceComputer = "Компьютер";

    /// <summary>
    ///     Просьба уточнить модель телефона
    /// </summary>
    public const string SpecifyPhoneModel = "Уточните модель вашего телефона:";

    /// <summary>
    ///     Вариант ОС Windows
    /// </summary>
    public const string OSWindows = "OC Windows";

    /// <summary>
    ///     Вариант MacOS
    /// </summary>
    public const string OSMacOS = "MacOS";

    /// <summary>
    ///     Apple iPhone после 15 модели
    /// </summary>
    public const string AppleIphoneAfter15 = "Apple iPhone после 15 модели";

    /// <summary>
    ///     Apple iPhone до 15 модели
    /// </summary>
    public const string AppleIphoneBefore15 = "Apple iPhone до 15 модели";

    /// <summary>
    ///     Модель Samsung
    /// </summary>
    public const string Samsung = "Samsung";

    /// <summary>
    ///     Фото подключения + порядок (1)
    /// </summary>
    public const string ConnectionPhoto1 = "фото подключения + порядок";

    /// <summary>
    ///     Фото подключения + порядок (2)
    /// </summary>
    public const string ConnectionPhoto2 = "фото подключения + порядок";

    /// <summary>
    ///     Модель Honor
    /// </summary>
    public const string Honor = "Honor";

    /// <summary>
    ///     Модель Xiaomi
    /// </summary>
    public const string Xiaomi = "Xiaomi";

    /// <summary>
    ///     Модель Vivo
    /// </summary>
    public const string Vivo = "Vivo";

    /// <summary>
    ///     Модель Huawei
    /// </summary>
    public const string Huawei = "Huawei";

    /// <summary>
    ///     Вопрос о том, получилось ли подключить диктофон к компьютеру
    /// </summary>
    public const string DidConnectToComputer = "Получилось подключить диктофон к компьютеру?";

    /// <summary>
    ///     Модель POCO
    /// </summary>
    public const string Poco = "POCO";

    /// <summary>
    ///     Модель Redmi
    /// </summary>
    public const string Redmi = "Redmi";

    /// <summary>
    ///     Модель Realme
    /// </summary>
    public const string Realme = "Realme";

    /// <summary>
    ///     Успешное подключение
    /// </summary>
    public const string SuccessYes = "Да, получилось";

    /// <summary>
    ///     Неудачное подключение
    /// </summary>
    public const string SuccessNo = "Нет, не получилось";

    /// <summary>
    ///     Модель Tecno
    /// </summary>
    public const string Tecno = "Tecno";

    /// <summary>
    ///     Нет модели телефона в списке
    /// </summary>
    public const string NoPhoneModel = "Нет моей модели телефона";

    /// <summary>
    ///     Рекомендация по включению OTG
    /// </summary>
    public const string OtgActivationRecommendation = "Рекомендация по включению OTG";

    public const string NeededOtg = "Необходим адаптер otg lightning usb";


    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Poco (Xiaomi)
    /// </summary>
    public const string OtgActivationPoco =
        "Чтобы включить OTG на смартфонах Poco от Xiaomi, как правило, не требуется никаких дополнительных настроек. " +
        "Достаточно подключить OTG-совместимое устройство через специальный переходник (обычно USB Type-C на USB Type-A), и смартфон сам распознает новое подключение. " +
        "На экране появится уведомление о том, что было найдено новое устройство, и можно будет начать работу с ним. " +
        "Если ничего не происходит, возможно, смартфон не поддерживает OTG";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Vivo
    /// </summary>
    public const string OtgActivationVivo = "Чтобы включить OTG на смартфоне vivo, нужно:\n" +
                                            "1. Открыть «Настройки».\n" +
                                            "2. Найти раздел «Bluetooth и устройства» или воспользоваться строкой поиска в верхней части экрана и ввести слово «OTG». Система сама найдёт нужный раздел.\n" +
                                            "3. Активировать переключатель «OTG-устройство». Он должен изменить цвет и сместиться вправо.\n\n" +
                                            "В большинстве случаев после включения OTG не требуется никаких дополнительных действий. " +
                                            "Достаточно подключить нужное устройство с помощью специального OTG-переходника. " +
                                            "Смартфон автоматически распознает подключённое устройство и уведомит об этом сообщением на экране.";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Huawei
    /// </summary>
    public const string OtgActivationHuawei = "Включение OTG через настройки в Huawei:\n" +
                                              "1. Войдите в настройки вашего телефона.\n" +
                                              "2. Перейдите в раздел \"Подключенные устройства\" или \"Дополнительные функции\" (в зависимости от версии прошивки).\n" +
                                              "3. Найдите опцию \"OTG\" или \"USB OTG\" и попробуйте включить ее.\n" +
                                              "4. Перезагрузите ваш телефон после того, как включили функцию OTG";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Redmi
    /// </summary>
    public const string OtgActivationRedmi = "Чтобы включить OTG на Redmi, нужно:\n" +
                                             "1. Зайти в «Настройки».\n" +
                                             "2. Перейти в «Расширенные настройки».\n" +
                                             "3. Найти пункт, посвящённый OTG (он может называться просто «OTG», «OTG-соединение» или что-то подобное).\n" +
                                             "4. Рядом с ним увидеть переключатель. Перевести его в активное положение";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Realme
    /// </summary>
    public const string OtgActivationRealme = "В зависимости от версии Realme UI способ настройки отличается:\n" +
                                              "1. Realme UI 1.0 и выше: «Настройки» > Поиск «OTG соединение» > Включить OTG соединение.\n" +
                                              "2. Realme R, S, T, Go и U Edition: «Настройки» > Система > OTG соединение.\n\n" +
                                              "Также можно активировать OTG через настройки смартфона:\n" +
                                              "1. Найти на рабочем столе или в меню приложений значок «Настройки» (обычно в виде шестерёнки) и нажать на него.\n" +
                                              "2. Найти раздел «Bluetooth и устройства» или воспользоваться поиском: в верхней части экрана настроек найти строку поиска и ввести слово «OTG».\n" +
                                              "3. В найденном разделе увидеть переключатель с надписью «OTG-устройство» или просто «OTG» и перевести его в активное положение.\n\n" +
                                              "В зависимости от версии Android и фирменной оболочки Realme UI расположение нужного пункта в настройках может отличаться.";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Tecno
    /// </summary>
    public const string OtgActivationTecno = "Чтобы включить OTG на смартфоне Tecno, нужно:\n" +
                                             "1. Открыть настройки. Для этого зайти в главное меню телефона и найти иконку «Настройки» (обычно в виде шестерёнки).\n" +
                                             "2. Найти раздел «Bluetooth и устройства» или «Подключение устройств». Название раздела может отличаться в зависимости от версии Android.\n" +
                                             "3. Если прямого переключателя OTG нет, воспользоваться поиском в настройках. Для этого ввести в строку поиска «OTG» и выбрать соответствующий результат.\n" +
                                             "4. Найти переключатель или опцию с названием «OTG-устройство» и активировать его. Обычно это нужно сделать только один раз, в дальнейшем OTG будет работать автоматически.\n" +
                                             "5. Подключить USB-устройство через специальный OTG-переходник. Обычно после подключения на экране телефона появится уведомление о том, что новое устройство обнаружено.";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Samsung
    /// </summary>
    public const string OtgActivationSamsung = "Чтобы включить функцию OTG на телефоне Samsung, необходимо:\n" +
                                               "1. Подключить внешнее устройство к телефону через порт USB с помощью кабеля OTG или переходника.\n" +
                                               "2. Зайти в настройки телефона и найти раздел «Дополнительные настройки».\n" +
                                               "3. Включить функцию OTG.\n" +
                                               "4. Подтвердить подключение.";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Honor
    /// </summary>
    public const string OtgActivationHonor =
        "Чтобы включить функцию OTG на смартфоне Honor, выполните следующие шаги:\n" +
        "1. Перейдите в \"Настройки\" на вашем смартфоне Honor.\n" +
        "2. Прокрутите вниз и найдите раздел \"Дополнительные настройки\".\n" +
        "3. В разделе \"Дополнительные настройки\" найдите и выберите пункт \"OTG\".\n" +
        "4. Включите переключатель рядом с \"OTG\", чтобы включить функцию.";

    /// <summary>
    ///     Инструкция по включению OTG на смартфонах Xiaomi
    /// </summary>
    public const string OtgActivationXiaomi = "Чтобы включить OTG на Xiaomi, нужно:\n" +
                                              "1. Зайти в главное меню «Настройки» (обычно это иконка шестерёнки).\n" +
                                              "2. Найти пункт «Дополнительные настройки» или «Расширенные настройки» (название может варьироваться).\n" +
                                              "3. Внутри «Дополнительных настроек» найти пункт, связанный с OTG. Он может называться просто «OTG», «USB OTG», или «Подключение OTG». " +
                                              "В некоторых версиях MIUI эта функция может быть спрятана в разделе «Подключения» или «Беспроводные сети».\n" +
                                              "4. Активировать нужный пункт. Обычно это делается с помощью переключателя";

    /// <summary>
    ///     Информация о расположении папки диктофона на разных устройствах
    /// </summary>
    public const string DictaphoneFolderLocation =
        "В каждом телефоне папка с диктофоном может находится в разных разделах, например:\n" +
        "- usb-накопитель\n" +
        "- флеш накопитель\n" +
        "- проводник\n" +
        "- мои файлы\n\n" +
        "В папке диктофона будут два файла:\n" +
        "- Record (там будут записи, которые вы сделали)\n" +
        "- mreсset.txt";

    /// <summary>
    ///     Словарь с инструкциями по включению OTG для разных моделей телефонов
    /// </summary>
    public static readonly Dictionary<string, string> OtgActivationInstructions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SAMSUNG"] = OtgActivationSamsung,
        ["HONOR"] = OtgActivationHonor,
        ["XIAOMI"] = OtgActivationXiaomi,
        ["HUAWEI"] = OtgActivationHuawei,
        ["POCO"] = OtgActivationPoco,
        ["REDMI"] = OtgActivationRedmi,
        ["REALME"] = OtgActivationRealme,
        ["TECNO"] = OtgActivationTecno,
        ["VIVO"] = OtgActivationVivo
    };

    /// <summary>
    ///     Получает инструкцию по включению OTG для указанной модели телефона
    /// </summary>
    /// <param name="phoneModel">Модель телефона</param>
    /// <returns>Инструкция по включению OTG или null, если модель не найдена</returns>
    public static string GetOtgActivationInstruction(string phoneModel)
    {
        return OtgActivationInstructions.GetValueOrDefault(phoneModel)!;
    }
}