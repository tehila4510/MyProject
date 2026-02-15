using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common.Dto.Questions
{
    public class QuestionReviewDto
    {
        public int QuestionId { get; set; }

        //זה בחזרה מהשרת
        [Required(ErrorMessage = "This field is required")]
        public int? SelectedOptionId { get; set; }

        //אחרי הבדיקה
        public string? CorrectOptionId { get; set; }
        public bool? IsCorrect { get; set; }
        public double TimeToAnswerMs { get; set; }
    }
}
