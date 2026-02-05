using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto.Questions
{
    public class QuestionReviewDto
    {
        //
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int SelectedOptionId { get; set; }
        public int CorrectOptionId { get; set; }
        public bool IsCorrect { get; set; }
        public double TimeToAnswerMs { get; set; }
    }
}
