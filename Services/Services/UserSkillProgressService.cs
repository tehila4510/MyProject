using AutoMapper;
using Common;
using Common.Dto.UserProgress;
using Common.StaticData;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
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
        private readonly IRepository<Question> questionRepository;

        private readonly IMapper mapper;
        public UserSkillProgressService(IProgressRepository repository, IMapper mapper, IRepository<Question> questionRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.questionRepository = questionRepository;
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

        public async Task UpdateSkillProgress(int userId, int skillId, double score)
        {
            var p = await repository.GetById(userId, skillId);
        }

        private async Task UpdateMastery(int userId, int questionId)
        {
            var q = await questionRepository.GetById(questionId);
            if (q == null)
                throw new KeyNotFoundException($"Session with id {questionId} not found");
            var skill = q.SkillId;
        }


        private int MapMasteryToLevel(int mastery)
        {
            if (mastery < 30) return 1;
            if (mastery < 60) return 2;
            if (mastery < 85) return 3;
            return 4;
        }
    }
}
