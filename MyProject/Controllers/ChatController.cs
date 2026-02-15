using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IOpenAi _openAi;

        public ChatController(IOpenAi openAi)
        {
            _openAi = openAi;
        }

        [HttpGet]
        public async Task<IActionResult> Ask(string question)
        {
            try
            {
                var response = await _openAi.chatGpt(question);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
