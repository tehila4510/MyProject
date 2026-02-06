using AutoMapper;
using Common.Dto.Question;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class QuestionService : IService<QuestionDto>
    {
        private readonly IRepository<Question> repository;
        private readonly IMapper mapper;
        public QuestionService(IRepository<Question> repository, IMapper mapper)
        {
            this.mapper= mapper;
            this.repository= repository;
        }
        public async Task<QuestionDto> Add(QuestionDto item)
        {
            var question =await repository.AddItem(mapper.Map<Question>(item));
            return mapper.Map<QuestionDto>(question);

        }

        public async Task Delete(int id)
        {
            await repository.DeleteItem(id);
        }

        public async Task<List<QuestionDto>> GetAll()
        {
            var questions =await repository.GetAll();
            return mapper.Map<List<QuestionDto>>(questions);
        }

        public async Task<QuestionDto> GetById(int id)
        {
            var question =await repository.GetById(id);
            return mapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto> Update(int id, QuestionDto item)
        {
            var question =await repository.UpdateItem(id, mapper.Map<Question>(item));
            return mapper.Map<QuestionDto>(question);
        }
    }
}
