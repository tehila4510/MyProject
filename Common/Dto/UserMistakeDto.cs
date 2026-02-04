using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class UserMistakeDto
    {
        public int MistakeId { get; set; } // PK
        public int UserId { get; set; } // FK
        public int QuestionId { get; set; } // FK
        public string UserMistakeText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
