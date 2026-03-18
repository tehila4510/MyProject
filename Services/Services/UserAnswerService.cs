using AutoMapper;
using Common;
using Common.Dto.Questions;
using Common.Dto.UserProgress;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class UserAnswerService : IService<UserAnswerDto>, IAnswerService
    {
        private readonly IRepository<UserAnswer> repository;
        private readonly IRepository<Question> questionRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper mapper;
        public UserAnswerService(IRepository<UserAnswer> repository, IRepository<Question> questionRepository, IMemoryCache cache, IMapper mapper)
        {
            this.repository = repository;
            this.questionRepository = questionRepository;
            this._cache = cache;
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


        //--------------אלגוריתם-------------

        public async Task<QuestionReviewDto> SubmitAnswer(int userId, UserAnswerDto dto)
        {
            // שליפת ה-answer הקיים שנוצר ב-GetNextQuestion (שם נשמר זמן ההתחלה)
            var answer = await repository.GetById((int)dto.AnswerId);
            if (answer == null)
                throw new KeyNotFoundException("Answer record not found");

            var question = await questionRepository.GetById(dto.QuestionId);
            if (question == null)
                throw new KeyNotFoundException("Question not found");

            // חישוב זמן תשובה אמיתי - מזמן יצירת הרשומה ב-GetNextQuestion
            var answeredAt = DateTime.UtcNow;
            answer.TimeToAnswerMs = answeredAt - answer.AnsweredAt!.Value; // AnsweredAt נשמר ב-GetNextQuestion
            answer.AnsweredAt = answeredAt;

            answer.UserId = userId;
            answer.QuestionId = dto.QuestionId;
            answer.SessionId = dto.SessionId;
            answer.UserAnswerText = dto.UserAnswerText;
            answer.IsCorrect = question.Options
                .First(o => o.OptionId == dto.SelectedOptionId).IsCorrect;

            await repository.UpdateItem(answer.AnswerId, answer);

            // עדכון קאש
            var userAnswers = await GetUserAnswersFromCache(userId);
            var existing = userAnswers.FirstOrDefault(a => a.AnswerId == answer.AnswerId);
            if (existing != null) userAnswers.Remove(existing);
            userAnswers.Add(answer);
            _cache.Set($"UserAnswers_{userId}", userAnswers, TimeSpan.FromMinutes(30));

            // מציאת התשובה הנכונה להחזיר בחזרה ללקוח
            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

            return new QuestionReviewDto
            {
                QuestionId = dto.QuestionId,
                SelectedOptionId = dto.SelectedOptionId,
                CorrectOptionId = correctOption?.OptionId.ToString(),
                IsCorrect = answer.IsCorrect,
                TimeToAnswerMs = answer.TimeToAnswerMs.TotalMilliseconds
            };
        }


        //-------------פעולות עזר------------
        private async Task<List<UserAnswer>> GetUserAnswersFromCache(int userId)
        {
            string cacheKey = $"UserAnswers_{userId}";
            if (!_cache.TryGetValue(cacheKey, out List<UserAnswer> userAnswers))
            {
                userAnswers = await repository
                    .GetByCondition(a => a.UserId == userId)
                    .ToListAsync();

                _cache.Set(cacheKey, userAnswers, TimeSpan.FromMinutes(30));
            }
            return userAnswers;
        }
    }
}
