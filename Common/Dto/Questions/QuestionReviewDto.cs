using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common.Dto.Questions
{
    public class QuestionReviewDto
    {
        public bool? IsCorrect { get; set; }
        public string FeedbackMessage { get; set; }   
        public string CorrectAnswerText { get; set; } 
    }
}
