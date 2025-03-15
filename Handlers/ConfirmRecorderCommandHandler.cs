using IAndIFamilySupport.API.Commands;
using IAndIFamilySupport.API.Helpers;
using IAndIFamilySupport.API.Interfaces;
using IAndIFamilySupport.API.Repositories;
using IAndIFamilySupport.API.States;
using MediatR;
using Telegram.Bot;

namespace IAndIFamilySupport.API.Handlers;

public class ConfirmRecorderCommandHandler(
    ITelegramBotClient bot,
    IStateService stateService)
    : IRequestHandler<ConfirmRecorderCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmRecorderCommand request, CancellationToken cancellationToken)
    {
        var callback = request.Update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;
        var userId = callback.From.Id;

        var state = stateService.GetUserState(userId);
        var previouslySelected = state.SelectedRecorderModel;
        var modelFromCallback = request.Model; // извлекается из RegEx @"^CONFIRM_RECORDER_(?<model>.+)$"

        if (previouslySelected != modelFromCallback)
        {
            // Пользователь что-то нажал не то
            await bot.SendMessage(
                chatId,
                $"Похоже, вы выбрали другую модель ({ModelHelper.GetUserFriendlyModelName(modelFromCallback)}).",
                cancellationToken: cancellationToken
            );
            await bot.SendMessage(
                chatId,
                "Давайте уточним модель ещё раз.",
                replyMarkup: KeyboardHelper.RecorderModels(),
                cancellationToken: cancellationToken
            );

            state.CurrentStep = ScenarioStep.SelectRecorderModel;
        }
        else
        {
            // Всё ок
            await bot.SendMessage(
                chatId,
                $"Отлично, модель {ModelHelper.GetUserFriendlyModelName(modelFromCallback)} выбрана.",
                cancellationToken: cancellationToken
            );

            // Предлагаем следующий шаг
            await bot.SendMessage(
                chatId,
                StartScenarioTextRepository.ChooseProblemPrompt,
                replyMarkup: KeyboardHelper.ProblemMenu(),
                cancellationToken: cancellationToken
            );

            // Смена шага
            state.CurrentStep = ScenarioStep.SelectProblem;
        }

        stateService.UpdateUserState(state);

        // Не забываем ответить на коллбек
        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}