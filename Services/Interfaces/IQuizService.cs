using Common.Dto.Question;
using Common.Dto.Questions;
using Common.Dto.Sessions;
using Common.Dto.UserProgress;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IQuizService
    {
        Task<int> StartSession(int userId);
        Task<SessionDto> EndSession(int sessionId);
        Task<QuestionDto> GetNextQuestion(int userId, int sessionId, int? skillId);
        Task<QuestionReviewDto> SubmitAnswer(int userId, UserAnswerDto dto);
    }
}
