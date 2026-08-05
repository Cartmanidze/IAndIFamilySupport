using System.ComponentModel.DataAnnotations;

namespace IAndIFamilySupport.API.Options;

public class TelegramSettings : IValidatableObject
{
    public string Token { get; set; } = string.Empty;

    public string WebhookUrl { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Token))
            yield return new ValidationResult(
                "Telegram bot token is required.",
                [nameof(Token)]);

        if (!Uri.TryCreate(WebhookUrl, UriKind.Absolute, out var webhookUri) ||
            webhookUri.Scheme != Uri.UriSchemeHttps)
            yield return new ValidationResult(
                "Telegram webhook URL must be an absolute HTTPS URL.",
                [nameof(WebhookUrl)]);
    }
}
