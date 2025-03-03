using IAndIFamilySupport.API.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Interfaces;

/// <summary>
/// Общий интерфейс для стратегий обработки разных шагов диалога.
/// </summary>
public interface IScenarioStrategy
{
    /// <summary>
    /// Список шагов, которые эта стратегия умеет обрабатывать.
    /// Например: ScenarioStep.Start, ScenarioStep.SelectRecorderModel и т.д.
    /// </summary>
    IEnumerable<ScenarioStep> TargetSteps { get; }

    /// <summary>
    /// Обработка входящего текстового сообщения (UpdateType.Message).
    /// </summary>
    Task HandleMessageAsync(ITelegramBotClient bot, Message message, TelegramUserState state);

    /// <summary>
    /// Обработка нажатия кнопки (UpdateType.CallbackQuery).
    /// </summary>
    Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, TelegramUserState state);
}