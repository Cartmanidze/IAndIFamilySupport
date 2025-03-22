using System.Reflection;
using System.Text.RegularExpressions;
using IAndIFamilySupport.API.Attributes;
using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Extensions;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Routing;

public class CommandRouter
{
    private readonly List<(Regex Pattern, Type CommandType)> _callbackPatternRoutes = [];
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
        var user = update.ExtractUser();

        if (user == null)
            return null;

        var state = _stateService.GetOrAddUserState(user);

        if (update.Message?.Text != null)
        {
            var text = update.Message.Text;
            if (state.CurrentStep == ScenarioStep.TransferToSupport) return new SupportMessageCommand(update.Message);

            if (_messageRoutes.TryGetValue(text, out var cmdType))
                return Activator.CreateInstance(cmdType, update.Message!, update.CallbackQuery) as IRequest<Unit>;
            if (state.CurrentStep == ScenarioStep.Start) return new StartCommand(update.Message!, update.CallbackQuery);
        }
        else if (update.BusinessMessage?.Text != null)
        {
            var text = update.BusinessMessage.Text;
            if (state.CurrentStep == ScenarioStep.TransferToSupport)
                return new SupportMessageCommand(update.BusinessMessage);

            if (_messageRoutes.TryGetValue(text, out var cmdType))
                return Activator.CreateInstance(cmdType, update.BusinessMessage!, update.CallbackQuery) as
                    IRequest<Unit>;

            if (state.CurrentStep == ScenarioStep.Start)
                return new StartCommand(update.BusinessMessage!, update.CallbackQuery);
        }
        else if (update.CallbackQuery?.Data != null)
        {
            var data = update.CallbackQuery.Data;

            if (state.CurrentStep == ScenarioStep.TransferToSupport)
                return new SupportMessageCommand(update.CallbackQuery!.Message!);

            if (state.CurrentStep == ScenarioStep.TransferToSupport)
                return new SupportCallbackCommand(update.CallbackQuery!.Message!, update.CallbackQuery);
            if (_callbackRoutes.TryGetValue(data, out var cmdType))
                return Activator.CreateInstance(cmdType, update.CallbackQuery!.Message!, update.CallbackQuery) as
                    IRequest<Unit>;

            foreach (var (pattern, patternCommandType) in _callbackPatternRoutes)
            {
                var match = pattern.Match(data);
                if (!match.Success) continue;
                if (!match.Groups["model"].Success)
                    return Activator.CreateInstance(patternCommandType, update.CallbackQuery!.Message!,
                        update.CallbackQuery) as IRequest<Unit>;
                var model = match.Groups["model"].Value;
                return Activator.CreateInstance(patternCommandType, update.CallbackQuery!.Message!,
                    update.CallbackQuery, model) as IRequest<Unit>;
            }
        }

        return null;
    }
}