using IAndIFamilySupport.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace IAndIFamilySupport.API.Controllers;

[ApiController]
[Route("telegram")]
public class TelegramController(ITelegramUpdateService telegramService) : ControllerBase
{
    [HttpPost("update")]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        await telegramService.HandleUpdateAsync(update);
        return Ok();
    }
}