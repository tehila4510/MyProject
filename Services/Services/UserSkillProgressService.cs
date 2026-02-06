using AutoMapper;
using Common.Dto.UserProgress;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Services.Services
{
    public class UserSkillProgressService : IService<UserSkillProgressDto>
    {
        private readonly IRepository<UserSkillProgress> repository;
        private readonly IMapper mapper;
        public UserSkillProgressService(IRepository<UserSkillProgress> repository, IMapper mapper)
        {
            this.repository= repository;
            this.mapper= mapper;
        }
        public async Task<UserSkillProgressDto> Add(UserSkillProgressDto item)
        {
           var userSkillProgress = await repository.AddItem(mapper.Map<UserSkillProgress>(item));
            return mapper.Map<UserSkillProgressDto>(userSkillProgress);
        }

        public async Task Delete(int id)
        {
            await repository.DeleteItem(id);
        }

        public async Task<List<UserSkillProgressDto>> GetAll()
        {
            var userSkillProgresses = await repository.GetAll();
            return mapper.Map<List<UserSkillProgressDto>>(userSkillProgresses);
        }

        public async Task<UserSkillProgressDto> GetById(int id)
        {
           var userSkillProgress =await repository.GetById(id);
            return mapper.Map<UserSkillProgressDto>(userSkillProgress);
        }

        public async Task<UserSkillProgressDto> Update(int id, UserSkillProgressDto item)
        {
            var usp=await repository.UpdateItem(id, mapper.Map<UserSkillProgress>(item));
            return mapper.Map<UserSkillProgressDto>(usp);
        }
    }
}
