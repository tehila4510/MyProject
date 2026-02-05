using Common.Dto.UserProgress;
using DataContext.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

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
        public async Task<List<UserSkillProgressDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<UserSkillProgressController>/5
        [HttpGet("{id}")]
        public async Task<UserSkillProgressDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<UserSkillProgressController>
        [HttpPost]
        public async Task<UserSkillProgressDto> Post([FromForm] UserSkillProgressDto value)
        {
            return await service.Add(value);
        }

        // PUT api/<UserSkillProgressController>/5
        [HttpPut("{id}")]
        public async Task<UserSkillProgressDto> Put(int id, [FromBody] UserSkillProgressDto value)
        {
            return await service.Update(id, value);
        }

        // DELETE api/<UserSkillProgressController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
