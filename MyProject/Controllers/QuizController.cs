using Common;
using Common.Dto.Question;
using Common.Dto.Questions;
using Common.Dto.UserProgress;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Services.Interfaces;
using Services.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IQuizService service;
        public QuizController(IConfiguration configuration, IQuizService service)
        {
            _configuration = configuration;
            this.service = service;
        }

        [HttpPost("start-session/{userId}")]
        public async Task<ActionResult<int>> StartSession(int userId)
        {
            try
            {
                var session = await service.StartSession(userId);
                return Ok(session);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("end-session/{sessionId}")]
        public async Task<IActionResult> EndSession(int sessionId)
        {
            try
            {
                await service.EndSession(sessionId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("next-question/{userId}/{sessionId}/{skillId?}")]
        public async Task<ActionResult<QuestionDto>> GetNextQuestion(int userId,  int sessionId, int? skillId)
        {
            try
            {
                var questionDto = await service.GetNextQuestion(userId,sessionId, skillId);

                if (questionDto == null)
                    return NotFound();

                return Ok(questionDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [HttpPost("submit-answer")]
        public async Task<ActionResult<QuestionReviewDto>> SubmitAnswer(int userId, [FromBody] UserAnswerDto dto)
        {
            try
            {
                var review = await service.SubmitAnswer(userId, dto);
                return Ok(review);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
