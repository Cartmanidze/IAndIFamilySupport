namespace IAndIFamilySupport.API.States;

public enum ScenarioStep
{
    Start = 0,               // /start
    SelectRecorderModel,     // Выбор модели (R8 PLUS, R3, R8)
    SelectProblem,           // Как подключить, Не воспроизводится, Помощь...
    HowToConnectDevice,      // К какому устройству пытаетесь подключить?
    HowToConnectPhoneModel,  // Уточняем модель телефона
    ConfirmPhoneConnected,   // "Получилось подключить к телефону?"
    ConfirmPcConnected,      // "Получилось подключить к ПК?"
    NotPlayingMenu,          // Не воспроизводится запись
    SettingsMenu,            // Помощь в настройке
    VoiceActivation,         // Настройка голосовой активации
    OtherSettings,           // Изменение MRECSET
    TransferToSupport,       // Передача на специалиста
    Finish                   // Сценарий завершён
}