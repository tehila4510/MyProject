using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Repository.Entities
{
    public class UserAnswer
    {
        //טבלה ששומרת בהתחלה את כל התשובות גם הנכונות וגם השגויות
        //מחשבת ציון וסטטיסטיקות
        //משאירה רק את התשובות השגויות
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
