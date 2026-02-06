using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataContext.Entities
{
    public class UserAnswer
    {
        //מתי זה מתרוקן?
        //אני צריכה להחליט
        //או בסוף כל שיעור אחרי שהוא חוזר על הטעויות
        //או שזה נשמר כל עוד לא תיקן וחזר על הטעות שלו והצליח
        [Key]
        public int AnswerId { get; set; } // PK
        public int UserId { get; set; } // FK
        public User User { get; set; }
        public int QuestionId { get; set; } // FK
        public Question Question { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }

        public string UserAnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public TimeSpan TimeToAnswerMs { get; set; }
    }
}
