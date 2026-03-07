using AutoMapper;
using Common;
using Common.Dto.UserProgress;
using Common.StaticData;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Services.Services
{
    public class UserSkillProgressService : IProgressService
    {
        private readonly IProgressRepository repository;
        private readonly IMapper mapper;
        public UserSkillProgressService(IProgressRepository repository, IMapper mapper)
        {
            this.repository= repository;
            this.mapper= mapper;
        }
        public async Task<UserSkillProgressDto> Add(UserSkillProgressDto item)
        {
           var userSkillProgress = await repository.AddItem(mapper.Map<UserSkillProgress>(item));
            return mapper.Map<UserSkillProgressDto>(userSkillProgress);
        }

        public async Task Delete(int userId, int skillId)
        {
            var userSkillProgress = await repository.GetById(userId,skillId);
            if (userSkillProgress == null)
                throw new KeyNotFoundException($"UserSkillProgress with id {userSkillProgress.UserSkillProgressId} not found");
            await repository.DeleteItem(userId, skillId);
        }

        public async Task<List<UserSkillProgressDto>> GetAll()
        {
            var userSkillProgresses = await repository.GetAll();
            if (userSkillProgresses == null || userSkillProgresses.Count == 0)
                throw new NotFoundException("No user skill progress found");
            return mapper.Map<List<UserSkillProgressDto>>(userSkillProgresses);
        }

        public async Task<UserSkillProgressDto> GetById(int userId,int skillId)
        {
            var userSkillProgress = await repository.GetById(userId, skillId);
            if (userSkillProgress == null)
                throw new KeyNotFoundException($"UserSkillProgress with id {userSkillProgress.UserSkillProgressId} not found");

            return mapper.Map<UserSkillProgressDto>(userSkillProgress);
        }

        public async Task<UserSkillProgressDto> Update(int userId, int skillId, UserSkillProgressDto item)
        {
            var userSkillProgress = await repository.GetById(userId, skillId);
            if (userSkillProgress == null)
                throw new KeyNotFoundException($"UserSkillProgress with id {userSkillProgress.UserSkillProgressId} not found");

            var usp =await repository.UpdateItem( userId,  skillId, mapper.Map<UserSkillProgress>(item));
            return mapper.Map<UserSkillProgressDto>(usp);
        }
    }
}
