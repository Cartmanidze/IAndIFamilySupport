using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands.Base;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Commands;

/// <summary>
///     Пользователь начинает настройку "других" (BIT, GAIN, PART, TIME),
///     учитывая выбранную модель (state.SelectedRecorderModel).
/// </summary>
[CallbackRoute("DO_OTHER_SETTINGS")]
public record OtherSettingsCommand(Message Message, CallbackQuery? CallbackQuery) : BaseCommand(Message, CallbackQuery);