using Common.Dto.Question;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService<QuestionDto> service;

        public QuestionController(IConfiguration _configuration, IService<QuestionDto> service)
        {
            this._configuration=_configuration;
            this.service=service;
        }
        // GET: api/<QuestionController>
        [HttpGet]
        public async Task<List<QuestionDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<QuestionController>/5
        [HttpGet("{id}")]
        public async Task<QuestionDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<QuestionController>
        [HttpPost]
        public async Task<QuestionDto> Post([FromForm] QuestionDto value)
        {
            return await service.Add(value);
        }

        // PUT api/<QuestionController>/5
        [HttpPut("{id}")]
        public async Task<QuestionDto> Put(int id, [FromBody] QuestionDto value)
        {
            return await service.Update(id,value);
        }

        // DELETE api/<QuestionController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
