using System.Text;
using Cronos;
using IAndIFamilySupport.API.Constants;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Tasks;

public class DailyReportService(IStateService stateService, ITelegramBotClient botClient) : BackgroundService
{
    private readonly CronExpression _cronExpression = CronExpression.Parse("0 1 * * *");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = _cronExpression.GetNextOccurrence(DateTime.UtcNow);
            if (nextRun.HasValue)
            {
                var delay = nextRun.Value - DateTime.UtcNow;
                await Task.Delay(delay, stoppingToken);
                await SendDailyReport(stoppingToken);
            }
            else
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task SendDailyReport(CancellationToken cancellationToken)
    {
        var report = GenerateReport();
        await SendReportToTelegram(report, cancellationToken);
    }

    private string GenerateReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Ежедневный отчёт за " + DateTime.Now.ToString("dd.MM.yyyy"));

        var allStates = stateService.GetAndClearUserStates().ToList();

        var problemCounts = allStates.Where(s => !string.IsNullOrEmpty(s.SelectedProblem))
            .GroupBy(s => s.SelectedProblem)
            .ToDictionary(g => g.Key!, g => g.Count());

        sb.AppendLine("\nКоличество проблем по типам:");
        foreach (var problem in problemCounts) sb.AppendLine($"- {TranslateProblem(problem.Key)}: {problem.Value}");

        var successful = allStates.Count(s => s.CurrentStep == ScenarioStep.Finish);
        sb.AppendLine($"\nУспешно обработано ботом: {successful}");

        var notFinished = allStates.Count(s =>
            s.CurrentStep != ScenarioStep.Finish && s.CurrentStep != ScenarioStep.TransferToSupport);

        sb.AppendLine($"\nНезавершённые: {notFinished}");

        var transferred = allStates.Count(s => s.CurrentStep == ScenarioStep.TransferToSupport);
        sb.AppendLine($"\nОтправлено на поддержку: {transferred}");

        sb.AppendLine("\nСписок пользователей:");
        foreach (var state in allStates)
            sb.AppendLine($"- Username: {state.Username ?? "Не указан"}, " +
                          $"Имя: {state.FirstName} {state.LastName ?? ""}, " +
                          $"Шаг: {TranslateStep(state.CurrentStep)}, " +
                          $"Модель диктофона: {state.SelectedRecorderModel ?? "Не выбрана"}, " +
                          $"Проблема: {TranslateProblem(state.SelectedProblem)}, " +
                          $"Устройство: {TranslateDevice(state.SelectedDevice)}, " +
                          $"Модель телефона: {state.SelectedPhoneModel ?? "Не выбрана"}");

        return sb.ToString();
    }

    private async Task SendReportToTelegram(string report, CancellationToken cancellationToken)
    {
        try
        {
            await botClient.SendMessage(MasterChat.MasterChatId, report, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке отчёта: {ex.Message}");
        }
    }

    private string TranslateProblem(string? problem)
    {
        return problem switch
        {
            "PROBLEM_CONNECT" => "Проблема с подключением",
            "PROBLEM_NOTPLAY" => "Не воспроизводится запись",
            "PROBLEM_SETTINGS" => "Помощь в настройке",
            _ => "Не выбрана"
        };
    }

    private string TranslateDevice(string? device)
    {
        return device switch
        {
            "PHONE" => "Телефон",
            "PC" => "Компьютер",
            _ => "Не выбрано"
        };
    }

    private static string TranslateStep(ScenarioStep step)
    {
        return step switch
        {
            ScenarioStep.Start => "Начало",
            ScenarioStep.SelectRecorderModel => "Выбор модели диктофона",
            ScenarioStep.ConfirmRecorderModel => "Подтверждение модели",
            ScenarioStep.SelectProblem => "Выбор проблемы",
            ScenarioStep.HowToConnectDevice => "Выбор устройства для подключения",
            ScenarioStep.HowToConnectPhoneModel => "Выбор модели телефона",
            ScenarioStep.HowToConnectPcModel => "Выбор модели ПК",
            ScenarioStep.ConfirmPhoneConnected => "Подтверждение подключения к телефону",
            ScenarioStep.ConfirmPcConnected => "Подтверждение подключения к ПК",
            ScenarioStep.NotPlayingMenu => "Меню \"Не воспроизводится\"",
            ScenarioStep.NotPlayingErrorType => "Тип ошибки воспроизведения",
            ScenarioStep.NotPlayingSolutionResult => "Результат решения проблемы",
            ScenarioStep.SettingsMenu => "Меню настроек",
            ScenarioStep.VoiceActivation => "Настройка голосовой активации",
            ScenarioStep.OtherSettings => "Другие настройки",
            ScenarioStep.TransferToSupport => "Передача на поддержку",
            ScenarioStep.Finish => "Завершение",
            _ => "Неизвестный шаг"
        };
    }
}