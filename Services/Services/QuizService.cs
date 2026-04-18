using AutoMapper;
using Common.Dto.Question;
using Common.Dto.Questions;
using Common.Dto.Sessions;
using Common.Dto.UserProgress;
using Microsoft.Extensions.Caching.Memory;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services.Services
{
    public class QuizService : IQuizService
    {
        private readonly IRepository<Session> sessionRepository;
        private readonly IRepository<User> userRepository;

        private readonly IRepository<Question> questionRepository;
        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;
        private readonly IProgressService progressService;
        public QuizService(
            IRepository<Question> questionRepository,
            IRepository<Session> sessionRepository,
            IQuestionService questionService,
            IAnswerService answerService,
            IProgressService progressService,
            IRepository<User> userRepository
           )
        {
            this.sessionRepository = sessionRepository;
            this.questionService = questionService;
            this.answerService = answerService;
            this.progressService = progressService;
            this.questionRepository = questionRepository;
            this.userRepository = userRepository;
        }
        public async Task<int> StartSession(int userId)
        {
            var session = new Session
            {
                UserId = userId,
                StartedAt = DateTime.UtcNow
            };

            var s = await sessionRepository.AddItem(session);
            return s.SessionId;
        }

        public async Task<SessionDto> EndSession(int sessionId)
        {
            var s = await sessionRepository.GetById(sessionId);
            if (s == null)
                throw new KeyNotFoundException($"Session with id {sessionId} not found");
            s.EndedAt = DateTime.UtcNow;
            
            double score = CalculateSessionScore(s.UserAnswers.ToList());
            s.Score = (int)Math.Round(score);
            var xp= CalculateXpGained(s.UserAnswers.ToList(), score);
            s.Xp = xp;
            s.TotalQuestions = total;
            s.CorrectAnswers = s.UserAnswers.Count(a => a.IsCorrect);

            // לרשום פה פונקציה של מחיקה של כל התשובות לשאלות הכפולות ולמחוק את כל התשובות הנכונות
            await sessionRepository.UpdateItem(sessionId, s);

            //עדכון היוזר
            var user = await userRepository.GetById(s.UserId);
            user.Xp += xp;
            user.CurrentLevel = user.Xp / 300; // לדוגמה, כל 100 XP = רמה חדשה
            user.Streak = user.LastActivity.HasValue && user.LastActivity.Value.Date == DateTime.UtcNow.Date.AddDays(-1) ? user.Streak + 1 : 1; 
            user.LastActivity = DateTime.UtcNow;
            await userRepository.UpdateItem(user.UserId, user);

            return new SessionDto
            {
                SessionId = s.SessionId,
                UserId = s.UserId,
                DurationInMinutes = s.StartedAt.HasValue && s.EndedAt.HasValue ? (s.EndedAt.Value - s.StartedAt.Value).TotalMinutes : null,
                Score = s.Score,
                Xp= xp,
            };
        }

        public async Task<QuestionDto> GetNextQuestion(int userId,int sessionId,int? skillId)
        {
            return await questionService.GetNextQuestion(userId, sessionId,skillId);
        }
        public async Task<QuestionReviewDto> SubmitAnswer(int userId, UserAnswerDto dto)
        {
            var review = await answerService.SubmitAnswer(userId, dto);
            var q= await questionRepository.GetById(dto.QuestionId);
            await progressService.UpdateSkillProgress(userId, q.SkillId, (bool)review.IsCorrect ? 1 : 0);
            return review;
        }


        //--------------פעולות עזר----------------
        int total = 0;
        private double CalculateSessionScore(List<UserAnswer> answers)
        {
            total = answers.Count;
            if (!answers.Any()) return 0;

            double totalWeighted = 0;
            double maxPossible = 0;

            foreach (var answer in answers)
            {
                var question = answer.Question;
                double levelMultiplier = question?.LevelId ?? 1;

                maxPossible += levelMultiplier * 1.0;

                if (!answer.IsCorrect)
                    continue;
                
                double timeWeight = GetTimeWeight(answer.TimeToAnswerMs);

                int attempts = answers.Count(a => a.QuestionId == answer.QuestionId);
                total-=attempts-1;
                double attemptWeight = GetAttemptWeight(attempts);

                totalWeighted += levelMultiplier * timeWeight * attemptWeight;
            }

            if (maxPossible == 0) return 0;

            return (totalWeighted / maxPossible) * 100;
        }
        // XP מחושב בנפרד - לפי ביצועים
        public int CalculateXpGained(List<UserAnswer> answers, double score)
        {
            int baseXp = 10; // XP בסיסי לסיום שיעור

            // בונוס לפי ציון
            int scoreBonus = score switch
            {
                >= 90 => 20,
                >= 70 => 10,
                >= 50 => 5,
                _ => 0
            };

            // בונוס אם ענה על שאלות קשות
            int hardQuestionBonus = answers
                .Where(a => a.IsCorrect && (a.Question?.LevelId ?? 0) >= 4)
                .Count() * 3;

            return baseXp + scoreBonus + hardQuestionBonus;
        }

        private double GetTimeWeight(TimeSpan time)
        {
            double seconds = time.TotalSeconds;

            if (seconds <= 2) return 1;
            if (seconds <= 5) return 0.8;
            if (seconds <= 10) return 0.6;

            return 0.4;
        }
        private double GetAttemptWeight(int attempts)
        {
            if (attempts == 1) return 1;
            if (attempts == 2) return 0.7;

            return 0.4;
        }

    }
}
