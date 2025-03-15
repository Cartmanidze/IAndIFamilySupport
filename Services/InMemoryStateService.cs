using System.Collections.Concurrent;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;

namespace IAndIFamilySupport.API.Services;

public class InMemoryStateService(ILogger<InMemoryStateService> logger) : IStateService
{
    // Используем ConcurrentDictionary вместо обычного Dictionary:
    private readonly ConcurrentDictionary<long, TelegramUserState> _userStates = new();

    public TelegramUserState GetUserState(long userId)
    {
        // Метод GetOrAdd безопасен для многопоточности и
        // возвращает существующее состояние или добавляет новое, если ключа нет
        return _userStates.GetOrAdd(
            userId,
            id => new TelegramUserState { UserId = id }
        );
    }

    public void UpdateUserState(TelegramUserState state)
    {
        // AddOrUpdate либо добавит запись, если её нет, либо обновит существующую
        _userStates.AddOrUpdate(
            state.UserId,
            state,                  // значение, если ещё не существует
            (_, _) => state // лямбда обновления
        );

        logger.LogInformation(
            "Обновлено состояние для пользователя {UserId}: Шаг={Step}",
            state.UserId,
            state.CurrentStep
        );
    }
}