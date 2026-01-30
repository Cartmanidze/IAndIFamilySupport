namespace IAndIFamilySupport.API.Repositories;

public static class AccessoriesCheckRepository
{
    /// <summary>
    ///     Если не появляется, то проверить аксессуары (фото/видео)
    /// </summary>
    public const string CheckAccessories = "Если не появляется папка с диктофоном, то проверить  аксессуары";

    /// <summary>
    ///     Примечание для iPhone 16 о расположении раздела аксессуаров
    /// </summary>
    public const string Iphone16AccessoriesNote =
        "На некоторых моделях iPhone 16 (в зависимости от версии iOS), раздел \"аксессуары\" находится в другом месте.";
}
