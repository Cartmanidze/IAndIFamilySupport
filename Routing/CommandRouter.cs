using System.Reflection;
using System.Text.RegularExpressions;
using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Extensions;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IAndIFamilySupport.API.Routing;

public class CommandRouter
{
    private readonly List<(Regex Pattern, Type CommandType)> _callbackPatternRoutes = new();
    private readonly Dictionary<string, Type> _callbackRoutes = new();
    private readonly Dictionary<string, Type> _messageRoutes = new();
    private readonly IStateService _stateService;

    public CommandRouter(IStateService stateService)
    {
        _stateService = stateService;
        RegisterFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void RegisterFromAssembly(Assembly assembly)
    {
        var types = assembly.GetTypes();

        foreach (var t in types)
        {
            // 1) Ищем [MessageRoute(...)]
            var msgRouteAttrs = t.GetCustomAttributes<MessageRouteAttribute>(false);
            foreach (var attr in msgRouteAttrs)
                _messageRoutes.TryAdd(attr.Text, t);

            // 2) Ищем [CallbackRoute(...)]
            var cbRouteAttrs = t.GetCustomAttributes<CallbackRouteAttribute>(false);
            foreach (var attr in cbRouteAttrs)
                _callbackRoutes.TryAdd(attr.Data, t);

            // 3) Ищем [CallbackRoutePattern(...)]
            var cbPatternAttrs = t.GetCustomAttributes<CallbackRoutePatternAttribute>(false);
            foreach (var patternAttr in cbPatternAttrs)
                _callbackPatternRoutes.Add((patternAttr.Pattern, t));
        }
    }

    public IRequest<Unit>? ResolveCommand(Update update)
    {
        // 1. Пытаемся определить userId
        var userData = update.ExtractChatAndUserId();

        var userId = userData.userId;

        if (userId == 0)
            // Не смогли определить пользователя — возможно, System message
            return null;

        // 2. Получаем текущее состояние
        var state = _stateService.GetUserState(userId);

        // 3. Если пользователь на шаге TransferToSupport — тогда обрабатываем 
        //    через "команды поддержки", а не обычный словарь.
        if (state.CurrentStep == ScenarioStep.TransferToSupport) return ResolveSupportCommand(update);

        // 4. Если пользователь НЕ в поддержке — работаем по обычным правилам

        // === Обработка обычных сообщений ===
        if (update.Message?.Text != null)
        {
            var text = update.Message.Text;
            if (_messageRoutes.TryGetValue(text, out var cmdType))
                return Activator.CreateInstance(cmdType, update) as IRequest<Unit>;
        }
        // === Обработка обычных коллбэков ===
        else if (update.CallbackQuery?.Data != null)
        {
            var data = update.CallbackQuery.Data;

            // Сначала точное совпадение
            if (_callbackRoutes.TryGetValue(data, out var cmdType))
                return Activator.CreateInstance(cmdType, update) as IRequest<Unit>;

            // Потом проверяем паттерны (регулярки)
            foreach (var (pattern, patternCommandType) in _callbackPatternRoutes)
            {
                var match = pattern.Match(data);
                if (match.Success)
                {
                    // Если есть именованная группа "model" (пример)
                    if (match.Groups["model"].Success)
                    {
                        var model = match.Groups["model"].Value;
                        return Activator.CreateInstance(patternCommandType, update, model) as IRequest<Unit>;
                    }

                    return Activator.CreateInstance(patternCommandType, update) as IRequest<Unit>;
                }
            }
        }

        // Если не нашли — вернём null
        return null;
    }

    /// <summary>
    ///     Если пользователь в поддержке (TransferToSupport),
    ///     смотрим, пришло ли сообщение или коллбэк, и создаём "Support..." команды.
    /// </summary>
    private IRequest<Unit>? ResolveSupportCommand(Update update)
    {
        return update.Type switch
        {
            // Если сообщение
            // Например, SupportMessageCommand
            // (передаём Update в конструктор)
            UpdateType.Message when update.Message != null => new SupportMessageCommand(update),
            // Если коллбэк
            // SupportCallbackCommand
            UpdateType.CallbackQuery when update.CallbackQuery != null => new SupportCallbackCommand(update),
            _ => null
        };
    }
}