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
    public class QuestionOptionService : IService<QuestionOptionDto>
    {
        private readonly IRepository<QuestionOption> repository;
        private readonly IMapper mapper;
        public QuestionOptionService()
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<QuestionOptionDto> Add(QuestionOptionDto item)
        {
            var qo=await repository.AddItem(mapper.Map<QuestionOption>(item));
            return mapper.Map<QuestionOptionDto>(qo);
        }

        public async Task Delete(int id)
        {
            await repository.DeleteItem(id);
        }

        public async Task<List<QuestionOptionDto>> GetAll()
        {
            var qo= await repository.GetAll();
            return mapper.Map<List<QuestionOptionDto>>(qo);
        }

        public async Task<QuestionOptionDto> GetById(int id)
        {
           var qo=await repository.GetById(id);
            return mapper.Map<QuestionOptionDto>(qo);
        }

        public Task<QuestionOptionDto> Update(int id, QuestionOptionDto item)
        {
            var qo = repository.UpdateItem(id, mapper.Map<QuestionOption>(item));
            return mapper.Map<Task<QuestionOptionDto>>(qo);
        }
    }
}
