using AutoMapper;
using Common;
using Common.Dto.Questions;
using Common.Dto.UserProgress;
using Microsoft.AspNetCore.Http;
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

        public  async Task<QuestionReviewDto> SubmitAnswer(int userId, UserAnswerDto dto)
        {
            var answer = mapper.Map<UserAnswer>(dto);
            answer.UserId = userId;
            answer.AnsweredAt = DateTime.UtcNow;
            answer.TimeToAnswerMs = (TimeSpan)(DateTime.UtcNow - answer.AnsweredAt);

            var question = await questionRepository.GetById(dto.QuestionId);
            answer.IsCorrect = question.Options.First(o => o.OptionId == dto.SelectedOptionId).IsCorrect;

            await repository.UpdateItem((int)dto.AnswerId, answer);

            // עדכון קאש
            var userAnswers = await GetUserAnswersFromCache(userId);
            userAnswers.Add(answer);
            _cache.Set($"UserAnswers_{userId}", userAnswers, TimeSpan.FromMinutes(30));

            // הכנת Review DTO
            var review = new QuestionReviewDto
            {
                QuestionId = dto.QuestionId,
                SelectedOptionId = dto.SelectedOptionId,
                IsCorrect = answer.IsCorrect,
                TimeToAnswerMs = answer.TimeToAnswerMs.TotalMilliseconds
            };
            return review;
        }

     

        //-------------פעולות עזר------------
        private async Task<List<UserAnswer>> GetUserAnswersFromCache(int userId)
        {
            string cacheKey = $"UserAnswers_{userId}";
            if (!_cache.TryGetValue(cacheKey, out List<UserAnswer> userAnswers))
            {
                var allAnswers = await repository.GetAll(); // await כאן
                userAnswers = allAnswers.Where(a => a.UserId == userId).ToList(); // ואז Where
                _cache.Set(cacheKey, userAnswers, TimeSpan.FromMinutes(30));
            }
            return userAnswers;
        }
    }
}
