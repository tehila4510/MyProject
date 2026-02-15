using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace Common.Dto.UserProgress
{
    public class UserAnswerDto
    {
        public int AnswerId { get; set; }  
        public int UserId { get; set; }
        [Required(ErrorMessage = "This field is required")]

        public int QuestionId { get; set; }
        [Required(ErrorMessage = "This field is required")]

        public int SessionId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string UserAnswerText { get; set; }

        public bool IsCorrect { get; set; }
        public int? SelectedOptionId { get; set; }
    }
}
