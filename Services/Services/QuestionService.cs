using AutoMapper;
using Common;
using Common.Dto.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class QuestionService : IService<QuestionDto>, IQuestionService
    {
        private readonly IRepository<Question> repository;
        private readonly IRepository<UserAnswer> answerRepository;
        private readonly IRepository<User> userRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper mapper;
        public QuestionService(IRepository<Question> repository, IMapper mapper, IRepository<UserAnswer> answerRepository, IMemoryCache cache,IRepository<User> userRepository)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.answerRepository = answerRepository;
            this.userRepository = userRepository;
            this._cache = cache;
        }
        #region CRUD Methods
        public async Task<QuestionDto> Add(QuestionDto dto)
        {
            var question = mapper.Map<Question>(dto);

            await repository.AddItem(question);

            var q = await repository.GetById(question.QuestionId);

            return mapper.Map<QuestionDto>(q);
        }

        public async Task Delete(int id)
        {
            var question = await repository.GetById(id);
            if (question == null)
            {
                throw new KeyNotFoundException($"Question with id {id} not found");
            }

            await repository.DeleteItem(id);
        }

        public async Task<List<QuestionDto>> GetAll()
        {
            var questions =await repository.GetAll();
            if (questions == null || questions.Count == 0)
                throw new NotFoundException("No questions found");
            return mapper.Map<List<QuestionDto>>(questions);
        }

        public async Task<QuestionDto> GetById(int id)
        {
            var question =await repository.GetById(id);
            if (question == null)
                throw new KeyNotFoundException($"Question with id {id} not found");
            return mapper.Map<QuestionDto>(question);
        }

       

        public async Task<QuestionDto> Update(int id, QuestionDto item)
        {
            var existingQuestion = await repository.GetById(id);
            if (existingQuestion == null)
                throw new KeyNotFoundException($"Question with id {id} not found");

            var question =await repository.UpdateItem(id, mapper.Map<Question>(item));
            return mapper.Map<QuestionDto>(question);
        }
        #endregion

        //------------Algorithm-------------

       
        public async Task<QuestionDto> GetNextQuestion(int userId, int sessionId, int? skillId)
        {
            var user = await userRepository.GetById(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            int userLevel = user.CurrentLevel;

            int resolvedSkillId = skillId
            ?? Random.Shared.Next(1, 9);

            var sessionAnsweredIds = await answerRepository
                .GetByCondition(a => a.SessionId == sessionId)
                .Select(a => a.QuestionId)
                .ToHashSetAsync();

            var availableQuestions = await repository
                .GetByCondition(q =>
                    q.SkillId == resolvedSkillId &&
                    !sessionAnsweredIds.Contains(q.QuestionId))
                .Include(q => q.Options)
                .ToListAsync();
            
            if (!availableQuestions.Any()) return null;

            var userAnswers = await GetUserAnswersFromCache(userId);
            var sessionAnswers = userAnswers.Where(a => a.SessionId == sessionId).ToList();
            int cntCorrectAns = GetCurrentCorrectAns(sessionAnswers);
            int targetLevel = CalculateTargetLevel(userLevel, cntCorrectAns);
            var selectedQuestion = SelectBestQuestion(availableQuestions, userAnswers, targetLevel);

            var answerRecord = new UserAnswer
            {
                UserId = userId,
                QuestionId = selectedQuestion.QuestionId,
                SessionId = sessionId,
                UserAnswerText = "",
                AnsweredAt = DateTime.UtcNow,
                TimeToAnswerMs = TimeSpan.Zero
            };
            await answerRepository.AddItem(answerRecord);

            var dto = mapper.Map<QuestionDto>(selectedQuestion);
            dto.AnswerRecordId = answerRecord.AnswerId;
            return dto;
        }
        private Question SelectBestQuestion(List<Question> available, List<UserAnswer> userHistory, int targetLevel)
        {
            var historyDict = userHistory
         .GroupBy(h => h.QuestionId)
         .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.AnsweredAt).First());

            var allWeighted = available.Select(q => new
            {
                Question = q,
                Weight = CalculateQuestionWeightOptimized(q, historyDict, targetLevel)
            })
            .OrderByDescending(x => x.Weight)
            .ToList();

            var topCandidates = allWeighted.Take(20).ToList();

            double totalWeight = topCandidates.Sum(x => x.Weight);
            double randomPoint = Random.Shared.NextDouble() * totalWeight;
            double currentSum = 0;

            foreach (var candidate in topCandidates)
            {
                currentSum += candidate.Weight;
                if (currentSum >= randomPoint)
                    return candidate.Question;
            }

            return topCandidates.First().Question;
        }
        private double CalculateQuestionWeightOptimized(Question question, Dictionary<int, UserAnswer> historyDict, int targetLevel)
        {
            double weight = 1.0;

            int levelDiff = Math.Abs(question.LevelId - targetLevel);
            weight += levelDiff switch { 0 => 3.0, 1 => 1.5, 2 => 0.5, _ => 0.0 };

            if (!historyDict.TryGetValue(question.QuestionId, out var lastAnswer))
            {
                weight += 1.5;
            }
            else
            {
                if (!lastAnswer.IsCorrect)
                    weight += 2.0;
                else
                    weight -= 0.7;
            }

            return weight;
        }
        #region Helper Methods
        private int GetCurrentCorrectAns(List<UserAnswer> sessionAnswers)
        {
            int streak = 0;
            foreach (var answer in sessionAnswers.OrderByDescending(a => a.AnsweredAt))
            {
                if (answer.IsCorrect)
                    streak++;
                else
                    break;
            }
            return streak;
        }

        private int CalculateTargetLevel(int userLevel, int correctStreak)
        {
            int streakBoost = correctStreak / 5;
            return Math.Min(userLevel + streakBoost, 6);
        }

        private async Task<List<UserAnswer>> GetUserAnswersFromCache(int userId)
        {
            // _cache.Remove($"UserAnswers_{userId}");
            string cacheKey = $"UserAnswers_{userId}";
            if (!_cache.TryGetValue(cacheKey, out List<UserAnswer> userAnswers))
            {
                userAnswers = await answerRepository
                    .GetByCondition(a => a.UserId == userId)
                    .ToListAsync();

                _cache.Set(cacheKey, userAnswers, TimeSpan.FromMinutes(30));
            }
            return userAnswers;
        }
        #endregion
    }
}
