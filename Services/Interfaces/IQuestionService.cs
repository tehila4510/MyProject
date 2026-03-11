using Common.Dto.Question;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionDto> GetNextQuestion(int userId, int sessionId, int? skillId);
    }
}
