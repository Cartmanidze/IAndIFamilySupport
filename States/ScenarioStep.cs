namespace IAndIFamilySupport.API.States;

public enum ScenarioStep
{
    Start = 0, // /start
    SelectRecorderModel, // Выбор модели (R8 PLUS, R3, R8)
    ConfirmRecorderModel, // Подтверждение выбранной модели
    SelectProblem, // Как подключить, Не воспроизводится, Помощь...
    HowToConnectDevice, // К какому устройству пытаетесь подключить?
    HowToConnectPhoneModel, // Уточняем модель телефона
    HowToConnectPcModel, // Уточняем модель компьютера
    ConfirmPhoneConnected, // "Получилось подключить к телефону?"
    ConfirmPcConnected, // "Получилось подключить к ПК?"
    NotPlayingMenu, // Не воспроизводится запис
    NotPlayingDeviceSelection, // Выбор устройства воспроизведения (телефон/компьютер)
    NotPlayingErrorType, // Тип ошибки (кодировка или другая)
    NotPlayingSolutionResult, // Результат предложенного решения
    SettingsMenu, // Помощь в настройке
    VoiceActivation, // Настройка голосовой активации
    OtherSettings, // Изменение MRECSET
    TransferToSupport, // Передача на специалиста
    Finish // Сценарий завершён
}