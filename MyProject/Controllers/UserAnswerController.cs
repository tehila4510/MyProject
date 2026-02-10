using Common;
using Common.Dto.UserProgress;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnswerController : ControllerBase
    {

        private IConfiguration _configuration;
        private readonly IService<UserAnswerDto> service;
        public UserAnswerController(IConfiguration _configuration, IService<UserAnswerDto> service)
        {
            this._configuration = _configuration;
            this.service = service;
        }

        // GET: api/<UserAnswerController>
        [HttpGet]
        public async Task<IActionResult>  Get()
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

        // GET api/<UserAnswerController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var ua = await service.GetById(id);
                return Ok(ua);
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

        // POST api/<UserAnswerController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UserAnswerDto value)
        {
            //יש מה להוסיף פה
            try
            {
                var v = await service.Add(value);
                return CreatedAtAction(nameof(Get), new { id = v.AnswerId }, v);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<UserAnswerController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserAnswerDto value)
        {
            try
            {
               var update= await service.Update(id, value);
                return Ok(update);
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

        // DELETE api/<UserAnswerController>/5
        [HttpDelete("{id}")]
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
