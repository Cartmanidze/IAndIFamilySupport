using System.Collections.Concurrent;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Services;

public class InMemoryStateService(ILogger<InMemoryStateService> logger) : IStateService
{
    private readonly ConcurrentDictionary<long, TelegramUserState> _userStates = new();

    public TelegramUserState GetOrAddUserState(User user)
    {
        var userId = user.Id;
        return _userStates.GetOrAdd(
            userId,
            id => new TelegramUserState
                { UserId = id, FirstName = user.FirstName, LastName = user.LastName, Username = user.Username }
        );
    }

    public TelegramUserState GetUserState(long userId)
    {
        return _userStates.GetValueOrDefault(userId)!;
    }

    public void UpdateUserState(TelegramUserState state)
    {
        _userStates.AddOrUpdate(
            state.UserId,
            state,
            (_, _) => state
        );

        logger.LogInformation(
            "Обновлено состояние для пользователя {UserId}: Шаг={Step}",
            state.UserId,
            state.CurrentStep
        );
    }
}