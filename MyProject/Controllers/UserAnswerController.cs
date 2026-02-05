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
        public UserAnswerController()
        {
            this._configuration = _configuration;
            this.service = service;
        }

        // GET: api/<UserAnswerController>
        [HttpGet]
        public async Task<List<UserAnswerDto>>  Get()
        {
            return await service.GetAll();
        }

        // GET api/<UserAnswerController>/5
        [HttpGet("{id}")]
        public async Task<UserAnswerDto> Get(int id)
        {   return await service.GetById(id);
        }

        // POST api/<UserAnswerController>
        [HttpPost]
        public async Task<UserAnswerDto> Post([FromForm] UserAnswerDto value)
        {
            //יש מה להוסיף פה
            return await service.Add(value);
        }

        // PUT api/<UserAnswerController>/5
        [HttpPut("{id}")]
        public async Task<UserAnswerDto> Put(int id, [FromBody] UserAnswerDto value)
        {
           return  await service.Update(id, value);
        }

        // DELETE api/<UserAnswerController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
