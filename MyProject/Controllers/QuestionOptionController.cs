using Common.Dto;
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
        private readonly IService<QusetionOptionDto> service;
        public QuestionOptionController(IConfiguration _configuration, IService<QusetionOptionDto> service)
        {
            this._configuration = _configuration;
            this.service = service;
        }
        // GET: api/<QuestionOptionController>
        [HttpGet]
        public async Task<List<QusetionOptionDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<QuestionOptionController>/5
        [HttpGet("{id}")]
        public async Task<QusetionOptionDto> Get(int id)
        {
           return await service.GetById(id);
        }

        // POST api/<QuestionOptionController>
        [HttpPost]
        public async Task<QusetionOptionDto> Post([FromForm] QusetionOptionDto value)
        {
            return await service.Add(value);
        }

        // PUT api/<QuestionOptionController>/5
        [HttpPut("{id}")]
        public async Task<QusetionOptionDto> Put(int id, [FromBody] QusetionOptionDto value)
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
