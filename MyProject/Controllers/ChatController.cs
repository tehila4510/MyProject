using Common.Dto.Chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskTeacher([FromBody] UserRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
                return BadRequest("Message cannot be empty");

            var result = await _chatService.AskTeacherAsync(request);

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}