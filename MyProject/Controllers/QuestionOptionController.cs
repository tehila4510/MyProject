using Common.Dto.Question;
using DataContext.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionOptionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService<QuestionOptionDto> service;
        public QuestionOptionController(IConfiguration _configuration, IService<QuestionOptionDto> service)
        {
            this._configuration = _configuration;
            this.service = service;
        }
        // GET: api/<QuestionOptionController>
        [HttpGet]
        public async Task<List<QuestionOptionDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<QuestionOptionController>/5
        [HttpGet("{id}")]
        public async Task<QuestionOptionDto> Get(int id)
        {
           return await service.GetById(id);
        }

        // POST api/<QuestionOptionController>
        [HttpPost]
        public async Task<QuestionOptionDto> Post([FromForm] QuestionOptionDto value)
        {
            return await service.Add(value);
        }

        // PUT api/<QuestionOptionController>/5
        [HttpPut("{id}")]
        public async Task<QuestionOptionDto> Put(int id, [FromBody] QuestionOptionDto value)
        {
            return await service.Update(id,value);
        }

        // DELETE api/<QuestionOptionController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.Delete(id);
        }
    }
}
