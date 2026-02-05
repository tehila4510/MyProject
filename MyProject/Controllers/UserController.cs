using Common.Dto.User;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private IConfiguration _configuration;
        private readonly IService<UserDto> service;
        public readonly IsExist<UserDto> isExist;
        public UserController(IConfiguration _configuration, IService<UserDto> service, IsExist<UserDto> isExist)
        {
            this._configuration = _configuration;
            this.service = service;
            this.isExist = isExist;
        }
        //להוסיף פונקציה של לוגין ורגיסטר

        // GET: api/<UserController>
        [HttpGet]
        public async Task<List<UserDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<UserDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<UserDto> Post([FromForm] UserDto value)
        {
            //לכתוב פה את הקריאה לפונקציה שעושה המרה
            return await service.Add(value);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<UserDto> Put(int id, [FromBody] UserDto value)
        {
            return await service.Update(id, value);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }

        //1. יצירת טוקן
        //2. private BabyDto GetCurrentUser()

    }
}
