using Common;
using Common.Dto.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
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

        private IConfiguration configuration;
        private readonly IService<UserDto> service;
        public readonly IsExist<UserDto> isExist;
        
        public UserController(IConfiguration _configuration, IService<UserDto> service,IsExist<UserDto> isExist)
        {
            this.configuration = _configuration;
            this.service = service;
            this.isExist = isExist;
        }
        //להוסיף פונקציה של רגיסטר


        // GET: api/<UserController>
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var u =await isExist.Exist(login);
            if (u != null)
                return Ok(new { Token = GenerateToken(u) });
            return Unauthorized("User does not exist");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserUpdateDto register)
        {
            // בדיקה אם המשתמש כבר קיים
            var exists = await isExist.Exist(new LoginDto { Email=register.Email,Password=register.PasswordHash});
            if (exists != null)
                return BadRequest("User already exists");


            // טיפול בקובץ אם יש Avatar
            if (register.file != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(register.file.FileName);
                var path = Path.Combine(Environment.CurrentDirectory, "ProfileImages/", fileName);

                using var fs = new FileStream(path, FileMode.Create);
                await register.file.CopyToAsync(fs);

                register.AvatarUrl = Encoding.UTF8.GetBytes(fileName);
            }

            // שמירה ב‑DB דרך Add הקיים
            //var createdUser = await service.Add(register);
            //// יצירת טוקן למשתמש חדש
            //var token = GenerateToken(createdUser);

            return Ok(/*new { Token = token, UserId = createdUser.UserId }*/);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDto value)
        {
            try { 
                // var updatedUser = await service.Update(id, value);
                return Ok(/*updatedUser*/);
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
            catch(Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        
        private string GenerateToken(UserDto u)
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
            new Claim(ClaimTypes.Name,u.Name),
             };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
