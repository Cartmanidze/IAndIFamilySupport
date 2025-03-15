using IAndIFamilySupport.API.Attributes;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь начинает настройку "других" (BIT, GAIN, PART, TIME),
///     учитывая выбранную модель (state.SelectedRecorderModel).
/// </summary>
[CallbackRoute("DO_OTHER_SETTINGS")]
public record OtherSettingsCommand(Update Update) : IRequest<Unit>;