using System;
using System.Collections.Generic;
using System.Text;

namespace DataContext.Entities
{
    public class Session
    {
        //זה אמור להיות באלגוריתם ולא בדטה
        public int SessionId { get; set; } // PK
        public int UserId { get; set; } // FK
        public User User { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int Score { get; set; }
        public int QuestionsCount { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    }
}
