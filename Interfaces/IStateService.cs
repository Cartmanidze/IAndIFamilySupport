using IAndIFamilySupport.API.States;

namespace IAndIFamilySupport.API.Interfaces;

public interface IStateService
{
    TelegramUserState GetUserState(long userId);

    void UpdateUserState(TelegramUserState state);
}