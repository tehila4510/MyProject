using Common;
using Common.Dto.Sessions;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Services.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService<SessionDto> service;
        public SessionController(IService<SessionDto> service, IConfiguration configuration)
        {
            this.service = service;
            _configuration = configuration;
        }
        // GET: api/<SessionController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var lst = await service.GetAll();
                return Ok(lst);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET api/<SessionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try { var s=await service.GetById(id);
                return Ok(s);
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

        // POST api/<SessionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] SessionDto value)
        {
            try
            {
                var v = await service.Add(value);
                return CreatedAtAction(nameof(Get), new { id = v.SessionId }, v);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        // PUT api/<SessionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SessionDto value)
        {
            try { 
                var updatedSession = await service.Update(id, value);
                return Ok(updatedSession);
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

        // DELETE api/<SessionController>/5
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
