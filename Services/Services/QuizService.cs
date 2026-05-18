using AutoMapper;
using Common.Dto.Question;
using Common.Dto.Questions;
using Common.Dto.Sessions;
using Common.Dto.UserProgress;
using Common.StaticData;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<UserAnswer> answerRepository;

        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;
        private readonly IProgressService progressService;
        public QuizService(
            IRepository<Question> questionRepository,
            IRepository<Session> sessionRepository,
            IQuestionService questionService,
            IAnswerService answerService,
            IProgressService progressService,
            IRepository<User> userRepository,
                IRepository<UserAnswer> answerRepository
           )
        {
            this.sessionRepository = sessionRepository;
            this.questionService = questionService;
            this.answerService = answerService;
            this.progressService = progressService;
            this.questionRepository = questionRepository;
            this.userRepository = userRepository;
            this.answerRepository = answerRepository;
        }
        public async Task<int> StartSession(int userId)
        {
            var openSessions = await sessionRepository
            .GetByCondition(s => s.UserId == userId && s.EndedAt == null)
            .ToListAsync();

            foreach (var open in openSessions)
            {
                open.EndedAt = open.StartedAt ?? DateTime.UtcNow; 
                open.Score = 0;
                open.TotalQuestions = 0;
                open.CorrectAnswers = 0;
                open.Xp = 0;
                await sessionRepository.UpdateItem(open.SessionId, open);

                var unanswered = open.UserAnswers
                    .Where(a => string.IsNullOrEmpty(a.UserAnswerText))
                    .Select(a => a.AnswerId)
                    .ToList();
                foreach (var id in unanswered)
                    await answerRepository.DeleteItem(id);
            }
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
            if (s.EndedAt.HasValue)
                return MapToSessionDto(s);

            s.EndedAt = DateTime.UtcNow;
            var completedAnswers = s.UserAnswers
           .Where(a => !string.IsNullOrEmpty(a.UserAnswerText))
           .ToList();

            var (score, totalQuestions, correctCount) = CalculateSessionScore(completedAnswers);
            int xp = CalculateXpGained(completedAnswers, score);

            s.Score = (int)Math.Round(score);
            s.TotalQuestions = totalQuestions;
            s.CorrectAnswers = correctCount;
            s.Xp = xp;

            await sessionRepository.UpdateItem(sessionId, s);
            await CleanupSessionAnswers(s);


            //עדכון היוזר
            var user = await userRepository.GetById(s.UserId);
            user.Xp += xp;
            user.CurrentLevel = CalculateLevel(user.Xp);
            user.Streak = CalculateStreak(user.LastActivity, user.Streak);
            user.LastActivity = DateTime.UtcNow;
            await userRepository.UpdateItem(user.UserId, user);

            return MapToSessionDto(s, xp);
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


        private async Task CleanupSessionAnswers(Session session)
        {
            var answersByQuestion = session.UserAnswers
                .Where(a => !string.IsNullOrEmpty(a.UserAnswerText))
                .GroupBy(a => a.QuestionId)
                .ToList();

            var idsToDelete = new List<int>();

            foreach (var group in answersByQuestion)
            {
                var ordered = group.OrderBy(a => a.AnsweredAt).ToList();
                var lastAnswer = ordered.Last();

                if (lastAnswer.IsCorrect)
                {
                    idsToDelete.AddRange(ordered.Select(a => a.AnswerId));
                }
                else
                {
                    idsToDelete.AddRange(ordered.Take(ordered.Count - 1).Select(a => a.AnswerId));
                }
            }

            var unansweredIds = session.UserAnswers
                .Where(a => string.IsNullOrEmpty(a.UserAnswerText))
                .Select(a => a.AnswerId);

            idsToDelete.AddRange(unansweredIds);

            foreach (var id in idsToDelete)
                await answerRepository.DeleteItem(id);
        }
        private (double score, int totalQuestions, int correctCount) CalculateSessionScore(List<UserAnswer> answers)
        {
            if (!answers.Any()) return (0, 0, 0);

            var questionGroups = answers
           .GroupBy(a => a.QuestionId)
           .ToList();

            int totalQuestions = questionGroups.Count;
            int correctCount = 0;
            double totalWeighted = 0;
            double maxPossible = 0;
            foreach (var group in questionGroups)
            {
                var orderedAttempts = group.OrderBy(a => a.AnsweredAt).ToList();
                var finalAnswer = orderedAttempts.Last(); 
                var question = finalAnswer.Question;

                double levelMultiplier = question?.LevelId ?? 1;
                maxPossible += levelMultiplier; 

                if (!finalAnswer.IsCorrect) continue;

                correctCount++;

                double timeWeight = GetTimeWeight(finalAnswer.TimeToAnswerMs);

                int attempts = orderedAttempts.Count;
                double attemptWeight = GetAttemptWeight(attempts);

                totalWeighted += levelMultiplier * timeWeight * attemptWeight;
            }

            double score = maxPossible == 0 ? 0 : (totalWeighted / maxPossible) * 100;
            return (score, totalQuestions, correctCount);
        }
        public int CalculateXpGained(List<UserAnswer> answers, double score)
        {
            int baseXp = 10; 

            int scoreBonus = score switch
            {
                >= 90 => 20,
                >= 70 => 10,
                >= 50 => 5,
                _ => 0
            };

            int hardQuestionBonus = answers
                .GroupBy(a => a.QuestionId)
                .Where(g =>
                {
                    var ordered = g.OrderBy(a => a.AnsweredAt).ToList();
                    return ordered.Last().IsCorrect              // נכון בסוף
                        && ordered.Count == 1                    // בניסיון ראשון
                        && (ordered.Last().Question?.LevelId ?? 0) >= 4;
                })
                .Count() * 5; 

            int streakBonus = 0; // ניתן להוסיף לוגיקה כאן

            return baseXp + scoreBonus + hardQuestionBonus;
        }

        private double GetTimeWeight(TimeSpan time)
        {
            double seconds = time.TotalSeconds;

            if (seconds <= 5) return 1.0;   
            if (seconds <= 15) return 0.9;  
            if (seconds <= 30) return 0.8;  

            return 0.7;                     
        }
        private double GetAttemptWeight(int attempts)
        {
            if (attempts == 1) return 1;
            if (attempts == 2) return 0.7;

            return 0.4;
        }
        private int CalculateLevel(int xp)
        {
            return Level.AllLevels
                .OrderByDescending(l => l.Key)
                .FirstOrDefault(l => xp >= l.Value.MinXp)
                .Key;
        }
        private int CalculateStreak(DateTime? lastActivity, int currentStreak)
        {
            if (!lastActivity.HasValue) return 1;

            var today = DateTime.UtcNow.Date;
            var lastDate = lastActivity.Value.Date;

            if (lastDate == today) return currentStreak;                 
            if (lastDate == today.AddDays(-1)) return currentStreak + 1; 
            return 1;                                                    
        }
        private SessionDto MapToSessionDto(Session s, int? xp = null) => new SessionDto
        {
            SessionId = s.SessionId,
            UserId = s.UserId,
            DurationInMinutes = s.StartedAt.HasValue && s.EndedAt.HasValue
           ? (s.EndedAt.Value - s.StartedAt.Value).TotalMinutes
           : null,
            Score = s.Score,
            Xp = xp ?? s.Xp,
        };
    }
}
