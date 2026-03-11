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
        private readonly IRepository<Question> questionRepository;
        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;
        private readonly IProgressService progressService;
        public QuizService(
            IRepository<Question> questionRepository,
            IRepository<Session> sessionRepository,
            IQuestionService questionService,
            IAnswerService answerService,
            IProgressService progressService)
        {
            this.sessionRepository = sessionRepository;
            this.questionService = questionService;
            this.answerService = answerService;
            this.progressService = progressService;
            this.questionRepository = questionRepository;
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
            await sessionRepository.UpdateItem(sessionId, s);
            return new SessionDto
            {
                SessionId = s.SessionId,
                UserId = s.UserId,
                DurationInMinutes = s.StartedAt.HasValue && s.EndedAt.HasValue ? (s.EndedAt.Value - s.StartedAt.Value).TotalMinutes : null,
                Score = s.Score
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
        private double CalculateSessionScore(List<UserAnswer> answers)
        {
            if (!answers.Any())
                return 0;

            double total = 0;

            foreach (var answer in answers)
            {
                if (!answer.IsCorrect)
                    continue;

                double timeWeight = GetTimeWeight(answer.TimeToAnswerMs);
                int attempts = answers.Count(a => a.QuestionId == answer.QuestionId);
                double attemptWeight = GetAttemptWeight(attempts);

                total += timeWeight * attemptWeight;
            }

            return (total / answers.Count) * 100;
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
