namespace IAndIFamilySupport.API.States;

public class TelegramUserState
{
    public long UserId { get; set; }

    public ScenarioStep CurrentStep { get; set; } = ScenarioStep.Start;

    // Модель диктофона (R8_PLUS / R3 / R8)
    public string? SelectedRecorderModel { get; set; }

    // Проблема (например, "PROBLEM_CONNECT" / "PROBLEM_NOTPLAY" / "PROBLEM_SETTINGS")
    public string? SelectedProblem { get; set; }

    // Тип устройства (PHONE / PC)
    public string? SelectedDevice { get; set; }

    // Конкретная модель телефона (XIAOMI, SAMSUNG и т.д.)
    public string? SelectedPhoneModel { get; set; }
}