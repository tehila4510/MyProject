using Common.Dto.Question;
using Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;
using Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionOptionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService<QuestionOptionDto> service;
        public QuestionOptionController(IConfiguration _configuration, IService<QuestionOptionDto> service)
        {
            this._configuration = _configuration;
            this.service = service;
        }
        // GET: api/<QuestionOptionController>
        [HttpGet]
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

        // GET api/<QuestionOptionController>/5
        [HttpGet("{id}")]
        public async Task<QuestionOptionDto> Get(int id)
        {
           return await service.GetById(id);
        }

        // POST api/<QuestionOptionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] QuestionOptionDto value)
        {
            try
            {
                var v = await service.Add(value);
                return CreatedAtAction(nameof(Get), new { id = v.OptionId }, v);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<QuestionOptionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] QuestionOptionDto value)
        {
            try
            {
                var updatedOption = await service.Update(id, value);
                return Ok(updatedOption);
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

        // DELETE api/<QuestionOptionController>/5
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
