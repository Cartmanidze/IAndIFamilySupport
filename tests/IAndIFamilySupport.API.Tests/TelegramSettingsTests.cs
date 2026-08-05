using System.ComponentModel.DataAnnotations;
using IAndIFamilySupport.API.Options;

namespace IAndIFamilySupport.API.Tests;

public class TelegramSettingsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validation_fails_when_token_is_missing(string? token)
    {
        var settings = CreateValidSettings();
        settings.Token = token!;

        var results = Validate(settings);

        Assert.Contains(results, result =>
            result.MemberNames.Contains(nameof(TelegramSettings.Token)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("http://example.com/telegram/update")]
    public void Validation_fails_when_webhook_is_not_https(string? webhookUrl)
    {
        var settings = CreateValidSettings();
        settings.WebhookUrl = webhookUrl!;

        var results = Validate(settings);

        Assert.Contains(results, result =>
            result.MemberNames.Contains(nameof(TelegramSettings.WebhookUrl)));
    }

    [Fact]
    public void Validation_succeeds_for_complete_https_settings()
    {
        var results = Validate(CreateValidSettings());

        Assert.Empty(results);
    }

    private static TelegramSettings CreateValidSettings()
    {
        return new TelegramSettings
        {
            Token = "123456:valid-token-placeholder",
            WebhookUrl = "https://bot.example.com/telegram/update"
        };
    }

    private static IReadOnlyCollection<ValidationResult> Validate(TelegramSettings settings)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            settings,
            new ValidationContext(settings),
            results,
            validateAllProperties: true);

        return results;
    }
}
