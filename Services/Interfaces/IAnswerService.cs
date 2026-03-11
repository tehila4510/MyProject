using Common.Dto.Questions;
using Common.Dto.UserProgress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAnswerService
    {
        Task<QuestionReviewDto> SubmitAnswer(int userId, UserAnswerDto dto);
    }
}
