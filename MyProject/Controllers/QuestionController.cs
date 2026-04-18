using Common;
using Common.Dto.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Services.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService<QuestionDto> service;
        

        public QuestionController(IConfiguration _configuration, IService<QuestionDto> service)
        {
            this._configuration = _configuration;
            this.service = service;
        }
        // GET: api/<QuestionController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var lst = await service.GetAll();
                return Ok(lst);
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

        // GET api/<QuestionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try {
                var userId = GetUserId();

                var q= await service.GetById(id);
                //if (!User.IsInRole("Admin") && q.LevelId > user.CurrentLevel)
                //       return Forbid("You do not have access to this question.");
                return Ok(q);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        private int GetUserId()
        {
            return int.Parse(User.FindFirst("userId")?.Value);
        }
        // POST api/<QuestionController>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Post([FromForm] QuestionDto value)
        {
            try
            {
                var v = await service.Add(value);
                return CreatedAtAction(nameof(Get), new { id = v.QuestionId }, v);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<QuestionController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Put(int id, [FromBody] QuestionDto value)
        {
            try { 
                var updatedQuestion = await service.Update(id, value);
                return Ok(updatedQuestion);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<QuestionController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
