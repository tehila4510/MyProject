using AutoMapper;
using Common;
using Common.Dto.UserProgress;
using Common.StaticData;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<Session> sessionRepository;
        private readonly IRepository<UserAnswer> answerRepository;

        private readonly IMapper mapper;
        public UserSkillProgressService(IProgressRepository repository, IMapper mapper, IRepository<Question> questionRepository, IRepository<User> userRepository, IRepository<Session> sessionRepository, IRepository<UserAnswer> answerRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.questionRepository = questionRepository;
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.answerRepository = answerRepository;   
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

        public async Task<List<UserSkillProgressViewDto>> GetByUser(int userId)
        {
            var progress = await repository
                .GetByCondition(s => s.UserId == userId)
                .ToListAsync();

            if (progress == null || progress.Count == 0)
                return new List<UserSkillProgressViewDto>();

            var sessions = await sessionRepository
                .GetByCondition(s => s.UserId == userId)
                .ToListAsync();

            var answers = await answerRepository
                .GetByCondition(a => a.UserId == userId)
                .Include(a => a.Question)
                .ToListAsync();

            return progress.Select(entity =>
            {
                var skillAnswers = answers
                    .Where(a => a.Question?.SkillId == entity.SkillId && !string.IsNullOrEmpty(a.UserAnswerText))
                    .ToList();

                int total = skillAnswers.Count;
                int correct = skillAnswers.Count(a => a.IsCorrect);
                int accuracy = total == 0 ? 0 : (int)Math.Round((double)correct / total * 100);

                return new UserSkillProgressViewDto
                {
                    SkillId = entity.SkillId,
                    SkillName = GetSkillName(entity.SkillId),
                    ProgressPercent = CalculateProgress(entity.Mastery),
                    Accuracy = accuracy,
                    WeeklyXp = GetWeeklyXp(sessions),
                    LastPracticed = entity.LastPracticed
                };
            }).ToList();
        }

        public async Task<UserSkillProgressViewDto> GetById(int userId,int skillId)
        {
            var entity = await repository.GetById(userId, skillId);
            if (entity == null)
                throw new KeyNotFoundException("UserSkillProgress not found");
            var sessions = await sessionRepository
              .GetByCondition(s => s.UserId == userId)
              .ToListAsync();

            var allAnswersForSkill = await answerRepository
        .GetByCondition(a => a.UserId == userId && a.Question.SkillId == skillId)
        .ToListAsync();

            int total = allAnswersForSkill.Count;
            int correct = allAnswersForSkill.Count(a => a.IsCorrect);
            return new UserSkillProgressViewDto
            {
                SkillId = entity.SkillId,
                SkillName = GetSkillName(entity.SkillId),

                ProgressPercent = CalculateProgress(entity.Mastery),
                Accuracy = CalculateAccuracy(total, correct),
                WeeklyXp = GetWeeklyXp(sessions),
                LastPracticed = entity.LastPracticed
            };
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
            if (user == null) throw new KeyNotFoundException("User not found");


            int newLevel = Level.AllLevels
               .OrderByDescending(l => l.Key)
               .FirstOrDefault(l => user.Xp >= l.Value.MinXp)
               .Key;

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

            int delta = isCorrect == 1 ? 5 : -3;
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

            DateTime today = DateTime.UtcNow.Date;
            int daysToSubtract = (int)today.DayOfWeek;
            DateTime startOfThisWeek = today.AddDays(-daysToSubtract);

            foreach (var session in sessions)
            {
                if (!session.StartedAt.HasValue) continue;

                var sessionDate = session.StartedAt.Value.Date;

                if (sessionDate >= startOfThisWeek && sessionDate <= today)
                {
                    int dayIndex = (int)sessionDate.DayOfWeek;
                    result[dayIndex] += session.Xp;
                }
            }

            return result.ToList();
        }
        private int CalculateAccuracy(int correctAnswers, int totalQuestions)
        {
            if (totalQuestions == 0)
                return 0;

            return (int)Math.Round((double)correctAnswers / totalQuestions * 100);
        }
        private string GetSkillName(int skillId)
        {
            return Skill.AllSkills.TryGetValue(skillId, out var skill)
                ? skill.Name
                : "Unknown";
        }

    }
}
