using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class UserAnswerDto
    {
        public int AnswerId { get; set; } // PK
        public int UserId { get; set; } // FK
        public int QuestionId { get; set; } // FK
        public string UserAnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
