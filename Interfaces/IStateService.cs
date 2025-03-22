using IAndIFamilySupport.API.States;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Interfaces;

public interface IStateService
{
    TelegramUserState? GetUserState(long userId);

    TelegramUserState GetOrAddUserState(User user);

    void UpdateUserState(TelegramUserState state);
}