using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto.Questions
{
    public class QuestionReviewDto
    {
        public int QuestionId { get; set; }

        //זה בחזרה מהשרת
        public int? SelectedOptionId { get; set; }

        //אחרי הבדיקה
        public string? CorrectOptionId { get; set; }
        public bool? IsCorrect { get; set; }
        public double TimeToAnswerMs { get; set; }
    }
}
