using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class QuestionOptionDto
    {
        public int OptionId { get; set; } // PK
        public int QuestionId { get; set; } // FK
        public string OptionText { get; set; }
        public bool? IsCorrect { get; set; } //האם נכון
    }

}
