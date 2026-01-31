using Common.Dto;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<List<SessionDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<SessionController>/5
        [HttpGet("{id}")]
        public async Task<SessionDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<SessionController>
        [HttpPost]
        public async Task<SessionDto> Post([FromForm] SessionDto value)
        {
            return await service.Add(value);
        }

        // PUT api/<SessionController>/5
        [HttpPut("{id}")]
        public async Task<SessionDto> Put(int id, [FromBody] SessionDto value)
        {
            return await service.Update(id, value);
        }

        // DELETE api/<SessionController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
