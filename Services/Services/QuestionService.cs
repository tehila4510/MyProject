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
            _cache.Remove($"UserAnswers_{userId}");
            string cacheKey = $"UserAnswers_{userId}";
            if (!_cache.TryGetValue(cacheKey, out List<UserAnswer> userAnswers))
            {
                // ✅ מסנן ישירות בDB - לא מושך הכל!
                userAnswers = await answerRepository
                    .GetByCondition(a => a.UserId == userId)
                    .ToListAsync();

                _cache.Set(cacheKey, userAnswers, TimeSpan.FromMinutes(30));
            }
            return userAnswers;
        }
        public async Task<QuestionDto> GetNextQuestion(int userId, int sessionId, int? skillId)
        {
            var user = await userRepository.GetById(userId);
            if (user == null) throw new KeyNotFoundException("User not found");
            int userLevel = user.CurrentLevel;

            skillId = skillId ??= Random.Shared.Next(1, 9);

            var sessionAnsweredIds = await answerRepository
                .GetByCondition(a => a.SessionId == sessionId)
                .Select(a => a.QuestionId)
                .ToHashSetAsync();

            var availableQuestions = await repository
                .GetByCondition(q =>
                    q.SkillId == skillId &&
                    !sessionAnsweredIds.Contains(q.QuestionId))
                .Include(q => q.Options)
                .ToListAsync();

            if (!availableQuestions.Any()) return null;

            var userAnswers = await GetUserAnswersFromCache(userId);
            var sessionAnswers = userAnswers.Where(a => a.SessionId == sessionId).ToList();
            int correctStreak = GetCurrentCorrectStreak(sessionAnswers);
            int targetLevel = CalculateTargetLevel(userLevel, correctStreak);

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


        // --- חישוב רצף נכונות בסשן הנוכחי ---
        private int GetCurrentCorrectStreak(List<UserAnswer> sessionAnswers)
        {
            // סופרים כמה תשובות אחרונות ברצף היו נכונות
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

        // --- חישוב רמת יעד לפי רצף ---
        private int CalculateTargetLevel(int userLevel, int correctStreak)
        {
            // כל 5 נכונות ברצף - עולים רמה אחת בקושי
            int levelBoost = correctStreak / 5;
            int targetLevel = userLevel + levelBoost;

            // לא נחרוג מהרמה המקסימלית (נניח max=5)
            return Math.Min(targetLevel, 5);
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

        // --- בחירת השאלה הטובה ביותר ---
        private Question SelectBestQuestion(
            List<Question> available,
            List<UserAnswer> userHistory,
            int targetLevel)
        {
            var weighted = available
                .Select(q => new
                {
                    Question = q,
                    Weight = CalculateQuestionWeight(q, userHistory, targetLevel)
                })
                .OrderByDescending(x => x.Weight)
                .ToList();

            // לא בוחרים תמיד את הראשון - קצת אקראיות בין Top 3
            // כדי שהחוויה לא תהיה צפויה מדי
            var topCandidates = weighted.Take(3).ToList();
            var randomIndex = new Random().Next(topCandidates.Count);
            return topCandidates[randomIndex].Question;
        }
        // --- חישוב משקל לשאלה ---
        private double CalculateQuestionWeight(
            Question question,
            List<UserAnswer> history,
            int targetLevel)
        {
            double weight = 1.0;

            // א. קירבה לרמת היעד - ככל שקרובה יותר, משקל גבוה יותר
            int levelDiff = Math.Abs(question.LevelId - targetLevel);
            weight += levelDiff switch
            {
                0 => 3.0,  // בדיוק הרמה הנכונה
                1 => 1.5,  // קרוב
                2 => 0.5,  // רחוק
                _ => 0.0   // רחוק מאוד
            };

            // ב. היסטוריה - שאלות שנכשל בהן → עדיפות גבוהה יותר (חזרה)
            var pastAnswer = history
                .Where(a => a.QuestionId == question.QuestionId)
                .OrderByDescending(a => a.AnsweredAt)
                .FirstOrDefault();

            if (pastAnswer == null)
                weight += 1.0; // שאלה חדשה - טובה ללמידה
            else if (!pastAnswer.IsCorrect)
                weight += 2.0; // נכשל → חשוב לחזור
            else
                weight -= 0.5; // כבר ידע - פחות דחוף

            return weight;
        }

        
    }
}
