using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;

namespace IAndIFamilySupport.API.Services;

public class InMemoryStateService : IStateService
{
    private readonly ILogger<InMemoryStateService> _logger;
    private readonly Dictionary<long, TelegramUserState> _userStates = new();

    public InMemoryStateService(ILogger<InMemoryStateService> logger)
    {
        _logger = logger;
    }

    public TelegramUserState GetUserState(long userId)
    {
        if (!_userStates.TryGetValue(userId, out var state))
        {
            state = new TelegramUserState { UserId = userId };
            _userStates[userId] = state;
        }

        return state;
    }

    public void UpdateUserState(TelegramUserState state)
    {
        _userStates[state.UserId] = state;
        _logger.LogInformation("Updated state for user {UserId}: Step={Step}",
            state.UserId, state.CurrentStep);
    }
}