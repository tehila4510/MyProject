using AutoMapper;
using Common;
using Common.Dto.Question;
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
        private readonly IMemoryCache _cache;
        private readonly IMapper mapper;
        public QuestionService(IRepository<Question> repository, IMapper mapper, IRepository<UserAnswer> answerRepository, IMemoryCache cache)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.answerRepository = answerRepository;
            this._cache = cache;
        }
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

        //------------אלגוריתם-------------

        private async Task<List<UserAnswer>> GetUserAnswersFromCache(int userId)
        {
            string cacheKey = $"UserAnswers_{userId}";
            if (!_cache.TryGetValue(cacheKey, out List<UserAnswer> userAnswers))
            {
                var allAnswers = await answerRepository.GetAll(); 
                userAnswers = allAnswers.Where(a => a.UserId == userId).ToList();
                _cache.Set(cacheKey, userAnswers, TimeSpan.FromMinutes(30));
            }
            return userAnswers;
        }

        public async Task<QuestionDto> GetNextQuestion(int userId, int sessionId,int? skillId)
        {
            Random random = new Random();
            skillId = (!skillId.HasValue)? random.Next(1,9) : skillId; //ברירת מחדל למי שלא שולח את הskillId
            var allQuestions = await repository.GetAll();
            var questions=allQuestions
                .Where(q => q.SkillId == skillId)
                .ToList();
            
            var userAnswers = await GetUserAnswersFromCache(userId);

            var answeredCorrectly = userAnswers.Where(a => a.IsCorrect).Select(a => a.QuestionId).ToHashSet();
            var sessionAnswered = userAnswers.Where(a => a.SessionId == sessionId).Select(a => a.QuestionId).ToHashSet();

            var possibleQuestions = questions
                .Where(q => !answeredCorrectly.Contains(q.QuestionId) &&
                            !sessionAnswered.Contains(q.QuestionId))
                .ToList();

            if (!possibleQuestions.Any())
            {
                possibleQuestions = questions.Where(q => !sessionAnswered.Contains(q.QuestionId)).ToList();
            }

            if (!possibleQuestions.Any())
                return null;

            var weightedQuestions = possibleQuestions
                .Select(q => new { Question = q, Weight = GetQuestionWeight(q, userAnswers) })
                .OrderByDescending(q => q.Weight)
                .ToList();


            //לשאלה הבאה- כבר מאתחלים את זמן התשובה
            UserAnswer answer = new UserAnswer
            {
                AnsweredAt = DateTime.UtcNow
            };
            await answerRepository.AddItem(answer);

            return mapper.Map<QuestionDto>(weightedQuestions.First().Question);
        }

        private double GetQuestionWeight(Question question, List<UserAnswer> answers)
        {
            var pastAnswer = answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
            double weight = 1.0;

            if (pastAnswer == null)
                weight = 1.0;
            else if (!pastAnswer.IsCorrect)
                weight = 1.2;
            else
                weight = 0.5;

            //קיבלתי רק את הלבל ID 
            weight *= question.LevelId;
            return weight;
        }
    }
}
