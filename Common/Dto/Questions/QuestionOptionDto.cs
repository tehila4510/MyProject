using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Dto.Question
{
    public class QuestionOptionDto
    {
        public int? OptionId { get; set; } // PK
        public int? QuestionId { get; set; } // FK

        [Required(ErrorMessage = "This field is required")]
        public string OptionText { get; set; }
        public bool? IsCorrect { get; set; }
    }

}
