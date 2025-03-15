using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Команда: пользователь выбирает, что настроить (голосовую активацию или другие параметры).
/// </summary>
[CallbackRoute("VOICE_ACTIVATION")]
[CallbackRoute("OTHER_SETTINGS")]
public record SelectSettingsOptionCommand(Update Update) : IRequest<Unit>;