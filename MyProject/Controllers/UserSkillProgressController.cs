using Common.Dto.UserProgress;
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
    public class UserSkillProgressController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService<UserSkillProgressDto> service;
        public UserSkillProgressController(IService<UserSkillProgressDto> service, IConfiguration configuration)
        {
            this.service = service;
            _configuration = configuration;
        }
        // GET: api/<UserSkillProgressController>
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

        // GET api/<UserSkillProgressController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try {var UserSkillProgress= await service.GetById(id);
                return Ok(UserSkillProgress);
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

        // POST api/<UserSkillProgressController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UserSkillProgressDto value)
        {
            try
            {
                var v = await service.Add(value);
                // id = v.SkillId - צריך לשנות
                return CreatedAtAction(nameof(Get), new { id = v.SkillId }, v);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<UserSkillProgressController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserSkillProgressDto value)
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

        // DELETE api/<UserSkillProgressController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try {
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
