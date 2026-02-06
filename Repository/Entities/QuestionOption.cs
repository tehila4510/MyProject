using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Repository.Entities
{
    public class QuestionOption
    {
        [Key]
        public int OptionId { get; set; } // PK
        public int QuestionId { get; set; } // FK
        public Question Question { get; set; }
        public string OptionText { get; set; } 
        public bool IsCorrect { get; set; } //האם נכון
    }
}
