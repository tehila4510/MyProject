using Common;
using Common.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService service;

        public UserController(IUserService _userService)
        {
            this.service = _userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var obj = await service.Login(dto);
                return Ok(obj);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserUpdateDto dto)
        {
            try
            {
                var obj = await service.Register(dto);
                return Ok(obj);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET api/<UserController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (!IsOwnerOrAdmin(id))
                    return Forbid();
                var user = await service.GetById(id);
                return Ok(user);
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

        // PUT api/<UserController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] UserUpdateDto value)
        {
            try {
                if (!IsOwnerOrAdmin(id))
                    return Forbid();
                var updatedUser = await service.Update(id, value);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/<UserController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!IsOwnerOrAdmin(id))
                    return Forbid();
                await service.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        private bool IsOwnerOrAdmin(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return currentUserId == id || User.IsInRole("Admin");
        }
    }
}
