using Common;
using Common.Dto.UserProgress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserSkillProgressController : ControllerBase
    {
        private readonly IProgressService service;
        public UserSkillProgressController(IProgressService service)
        {
            this.service = service;
        }
        // GET: api/<UserSkillProgressController>
        [Authorize(Roles = "Admin")]
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
        [Authorize]
        [HttpGet("{skillId}")]
        public async Task<IActionResult> Get(int skillId)
        {
            try {
                var userId = GetUserId();
                var UserSkillProgress = await service.GetById(userId,skillId);
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UserSkillProgressDto value)
        {
            try
            {
                var userId = GetUserId();
                value.UserId = userId;
                var v = await service.Add(value);
                return CreatedAtAction(nameof(Get), new { skillId = v.SkillId }, v);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<UserSkillProgressController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int skillId, [FromBody] UserSkillProgressDto value)
        {
            try
            {
                var userId = GetUserId();

                var update = await service.Update(userId,skillId, value);
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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int skillId)
        {
            try {
                var userId = GetUserId();
                await service.Delete(userId,skillId);
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

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
