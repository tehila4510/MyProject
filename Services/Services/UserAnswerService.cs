using AutoMapper;
using Common;
using Common.Dto.UserProgress;
using Microsoft.AspNetCore.Http;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class UserAnswerService : IService<UserAnswerDto>
    {
        private readonly IRepository<UserAnswer> repository;
        private readonly IMapper mapper;
        public UserAnswerService(IRepository<UserAnswer> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<UserAnswerDto> Add(UserAnswerDto item)
        {
            var ua = await repository.AddItem(mapper.Map<UserAnswer>(item));
            return mapper.Map<UserAnswerDto>(ua);
        }

        public async Task Delete(int id)
        {
            var ua = await repository.GetById(id);
            if (ua == null)
            {
                throw new KeyNotFoundException($"UserAnswer with id {id} not found");
            }
            await repository.DeleteItem(id);
        }

        public async Task<List<UserAnswerDto>> GetAll()
        {
            var ua = await repository.GetAll();
            if (ua==null || ua.Count==0)
                throw new NotFoundException("No user answers found");
            return mapper.Map<List<UserAnswerDto>>(ua);
        }

        public async Task<UserAnswerDto> GetById(int id)
        {
            var ua = await repository.GetById(id);
            if (ua == null)
                throw new KeyNotFoundException($"UserAnswer with id {id} not found");
            return mapper.Map<UserAnswerDto>(ua);
        }

        public async Task<UserAnswerDto> Update(int id, UserAnswerDto item)
        {
            var existingUa = await repository.GetById(id);
            if (existingUa == null)
                throw new KeyNotFoundException($"UserAnswer with id {id} not found");
            var ua = await repository.UpdateItem(id, mapper.Map<UserAnswer>(item));
            return mapper.Map<UserAnswerDto>(ua);
        }
    }
}
