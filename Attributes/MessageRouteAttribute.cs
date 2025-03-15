namespace IAndIFamilySupport.API.Attributes;

/// <summary>
///     Атрибут для маршрутизации по тексту сообщения.
///     Пример: [MessageRoute("/start")] => команда вызывается, если пользователь ввёл "/start".
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MessageRouteAttribute(string text) : Attribute
{
    public string Text { get; } = text;
}