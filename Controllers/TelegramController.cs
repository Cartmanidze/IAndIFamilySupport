using IAndIFamilySupport.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Controllers;

[ApiController]
[Route("telegram")]
public class TelegramController : ControllerBase
{
    private readonly ITelegramUpdateService _telegramService;

    public TelegramController(ITelegramUpdateService telegramService)
    {
        _telegramService = telegramService;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        await _telegramService.HandleUpdateAsync(update);
        return Ok();
    }
}