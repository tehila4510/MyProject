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
        private readonly IRepository<User> userRepository;

        private readonly IMapper mapper;
        public UserSkillProgressService(IProgressRepository repository, IMapper mapper, IRepository<Question> questionRepository, IRepository<User> userRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.questionRepository = questionRepository;
            this.userRepository = userRepository;
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
                throw new KeyNotFoundException("UserSkillProgress not found");
            await repository.DeleteItem(userId, skillId);
        }

        public async Task<List<UserSkillProgressDto>> GetAll()
        {
            var userSkillProgresses = await repository.GetAll();
            if (userSkillProgresses == null || userSkillProgresses.Count == 0)
                throw new NotFoundException("No user skill progress found");
            return mapper.Map<List<UserSkillProgressDto>>(userSkillProgresses);
        }

        public async Task<UserSkillProgressViewDto> GetById(int userId,int skillId)
        {
            var entity = await repository.GetById(userId, skillId);
            if (entity == null)
                throw new KeyNotFoundException("UserSkillProgress not found");

            return new UserSkillProgressViewDto
            {
                SkillId = entity.SkillId,
                SkillName = GetSkillName(entity.SkillId),

                ProgressPercent = CalculateProgress(entity.Mastery),
                // Accuracy = CalculateAccuracy(entity.CorrectAnswers, entity.TotalQuestions),

                //WeeklyXp = GetWeeklyXp(userId, skillId),


                Accuracy = 2,
                WeeklyXp = [500, 40, 30, 200],
                LastPracticed = entity.LastPracticed
            };
        }
        private string GetSkillName(int skillId)
        {
            return Skill.AllSkills.TryGetValue(skillId, out var skill)
                ? skill.Name
                : "Unknown";
        }
        public async Task<UserSkillProgressDto> Update(int userId, int skillId, UserSkillProgressDto item)
        {
            var userSkillProgress = await repository.GetById(userId, skillId);
            if (userSkillProgress == null)
                throw new KeyNotFoundException("UserSkillProgress not found");

            var usp =await repository.UpdateItem( userId,  skillId, mapper.Map<UserSkillProgress>(item));
            return mapper.Map<UserSkillProgressDto>(usp);
        }


        //--------------אלגוריתם----------------
        public async Task UpdateUserLevelAfterSession(int userId, double sessionScore)
        {
            var user = await userRepository.GetById(userId);
            if (user == null) return;

            // ממוצע XP צבור → רמה כללית
            // לדוגמה: כל 500 XP = רמה
            int newLevel = (user.Xp / 500) + 1;
            newLevel = Math.Clamp(newLevel, 1, 10);

            if (newLevel != user.CurrentLevel)
            {
                user.CurrentLevel = newLevel;
                await userRepository.UpdateItem(userId, user);
            }
        }

        public async Task UpdateSkillProgress(int userId, int skillId, int isCorrect)
        {
            var skillProgress = await repository.GetById(userId, skillId);

            if (skillProgress == null)
            {
                // יצירה ראשונית
                skillProgress = new UserSkillProgress
                {
                    UserId = userId,
                    SkillId = skillId,
                    Mastery = isCorrect == 1 ? 5 : 0,
                    LastPracticed = DateTime.UtcNow
                };
                await repository.AddItem(skillProgress);
                return;
            }

            // עדכון Mastery - בין 0 ל-100
            int delta = isCorrect == 1 ? 5 : -3; // נכון = +5, טעות = -3
            skillProgress.Mastery = Math.Clamp(skillProgress.Mastery + delta, 0, 100);
            skillProgress.LastPracticed = DateTime.UtcNow;

            await repository.UpdateItem(skillProgress.UserId, skillProgress.SkillId, skillProgress);
        }
        private int CalculateProgress(int mastery)
        {
            return Math.Clamp(mastery, 0, 100);
        }
        private List<int> GetWeeklyXp(IEnumerable<Session> sessions)
        {
            var result = new int[7];

            var startOfWeek = DateTime.UtcNow.Date.AddDays(-6);

            foreach (var session in sessions)
            {
                if (!session.StartedAt.HasValue)
                    continue;

                var date = session.StartedAt.Value.Date;

                if (date < startOfWeek)
                    continue;

                int dayIndex = (date - startOfWeek).Days;

                result[dayIndex] += session.Xp;
            }

            return result.ToList();
        }
        private int CalculateAccuracy(int correctAnswers, int totalQuestions)
        {
            if (totalQuestions == 0)
                return 0;

            return (int)Math.Round((double)correctAnswers / totalQuestions * 100);
        }
    }
}
