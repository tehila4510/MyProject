using AutoMapper;
using Common;
using Common.Dto.Questions;
using Common.Dto.Sessions;
using Common.Dto.UserProgress;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;

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
            if (ua == null || ua.Count == 0)
                throw new NotFoundException("No user answers found");
            return mapper.Map<List<UserAnswerDto>>(ua);
        }
        public async Task<List<UserAnswerDto>> GetByUser(int userId)
        { 
            var answers = await repository.GetByCondition(s => s.UserId == userId).ToListAsync();
            if (answers == null || answers.Count == 0)
                throw new NotFoundException("No answers found for the specified user");
            return mapper.Map<List<UserAnswerDto>>(answers);
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
            if (dto.UserId != userId)
                throw new UnauthorizedAccessException();

            var answer = await repository.GetById((int)dto.AnswerRecordId);
            if (answer == null)
                throw new KeyNotFoundException("Answer record not found");

            var question = await questionRepository.GetById(dto.QuestionId);
            if (question == null)
                throw new KeyNotFoundException("Question not found");

            var answeredAt = DateTime.UtcNow;
            answer.TimeToAnswerMs = answeredAt - answer.AnsweredAt!.Value;
            answer.UserId = userId;
            answer.QuestionId = dto.QuestionId;
            answer.SessionId = dto.SessionId;
            answer.UserAnswerText = (dto.SelectedOptionId.HasValue || dto.SelectedOptionId==0)
                ? question.Options.First(o => o.OptionId == dto.SelectedOptionId).OptionText
                : dto.UserAnswerText;

            answer.IsCorrect = CheckAnswer(question, dto);
            await repository.UpdateItem(answer.AnswerId, answer);

            // עדכון קאש
            var userAnswers = await GetUserAnswersFromCache(userId);
            var existing = userAnswers.FirstOrDefault(a => a.AnswerId == answer.AnswerId);
            if (existing != null) userAnswers.Remove(existing);
            userAnswers.Add(answer);
            _cache.Set($"UserAnswers_{userId}", userAnswers, TimeSpan.FromMinutes(30));

            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

            string feedback = (bool)answer.IsCorrect
                ? GetEncouragementMessage()
                : $"The correct answer is: {correctOption?.OptionText}";

            return new QuestionReviewDto
            {  
                IsCorrect = answer.IsCorrect,
                FeedbackMessage = feedback,
                CorrectAnswerText = (bool)answer.IsCorrect ? null : BuildCorrectAnswerText(question)
            };
        }


        //-------------פעולות עזר------------
        private string GetEncouragementMessage()
        {
            var messages = new[]
            {
              "Amazing! Keep it up! 🌟",
              "Excellent work! 🎉",
              "You're on fire! 🔥",
              "Perfect! You nailed it! 💪",
              "Brilliant! Well done! ⭐"
          };
            return messages[new Random().Next(messages.Length)];
        }

        private string BuildCorrectAnswerText(Question question)
        {
            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            if (correctOption == null) return string.Empty;

            // מילוי בתוך משפט - מחזיר את המשפט המלא
            if (question.HasQuestionType(QuestionTypeEnum.FillInTheBlank) ||
                question.HasQuestionType(QuestionTypeEnum.ClozeTest))
            {
                // "I ___ (to be) happy" → "I was happy"
                return question.QuestionText
                    .Replace("___", correctOption.OptionText)
                    .Replace($"({ExtractHint(question.QuestionText)})", "")
                    .Trim();
            }

            // טקסט חופשי - מציג רק את התשובה הנכונה
            if (question.HasQuestionType(QuestionTypeEnum.ShortAnswer) ||
                question.HasQuestionType(QuestionTypeEnum.Translate) ||
                question.HasQuestionType(QuestionTypeEnum.Spelling) ||
                question.HasQuestionType(QuestionTypeEnum.AudioResponse) ||
                question.HasQuestionType(QuestionTypeEnum.Pronunciation))
            {
                return correctOption.OptionText;
            }

            // בחירה רגילה - מציג את הטקסט של האופציה הנכונה
            if (question.HasQuestionType(QuestionTypeEnum.MultipleChoice) ||
                question.HasQuestionType(QuestionTypeEnum.TrueFalse) ||
                question.HasQuestionType(QuestionTypeEnum.ConversationCompletion) ||
                question.HasQuestionType(QuestionTypeEnum.PictureBased) ||
                question.HasQuestionType(QuestionTypeEnum.ImageLabeling) ||
                question.HasQuestionType(QuestionTypeEnum.Listening) ||
                question.HasQuestionType(QuestionTypeEnum.ReadingComprehension))
            {
                return correctOption.OptionText;
            }

            // מורכבים - מציג הסבר + תשובה
            if (question.HasQuestionType(QuestionTypeEnum.Ordering) ||
                question.HasQuestionType(QuestionTypeEnum.DragAndDrop))
            {
                return $"The correct order is: {correctOption.OptionText}";
            }

            if (question.HasQuestionType(QuestionTypeEnum.Matching))
            {
                return $"The correct match is: {correctOption.OptionText}";
            }

            if (question.HasQuestionType(QuestionTypeEnum.MultipleAnswer))
            {
                var allCorrect = question.Options
                    .Where(o => o.IsCorrect)
                    .Select(o => o.OptionText);
                return $"The correct answers are: {string.Join(", ", allCorrect)}";
            }

            return correctOption.OptionText;
        }

        // מחלץ את הרמז מתוך הסוגריים - (to be) → to be
        private string ExtractHint(string questionText)
        {
            var match = System.Text.RegularExpressions.Regex.Match(questionText, @"\(([^)]+)\)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
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

        private bool CheckAnswer(Question question, UserAnswerDto dto)
        {
            bool isOptionBased =
                question.HasQuestionType(QuestionTypeEnum.MultipleChoice) ||
                question.HasQuestionType(QuestionTypeEnum.TrueFalse) ||
                question.HasQuestionType(QuestionTypeEnum.ConversationCompletion) ||
                question.HasQuestionType(QuestionTypeEnum.PictureBased) ||
                question.HasQuestionType(QuestionTypeEnum.ImageLabeling);

            if (isOptionBased && dto.SelectedOptionId.HasValue)
            {
                return question.Options
                    .First(o => o.OptionId == dto.SelectedOptionId).IsCorrect;
            }

            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            if (correctOption == null) return false;

            return NormalizeText(dto.UserAnswerText) == NormalizeText(correctOption.OptionText);
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            return text
                .Trim()                    // רווחים מיותרים בהתחלה/סוף
                .ToLower()                 // אותיות גדולות/קטנות
                .Replace("  ", " ")       // רווחים כפולים באמצע
                .Replace("'", "'")        // גרשיים שונים (don't / don't)
                .Replace("'", "'");
        }

       
    }
}
