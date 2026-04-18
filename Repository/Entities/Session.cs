using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Entities
{
    public class Session
    {
        public int SessionId { get; set; } // PK
        public int UserId { get; set; } // FK
        public User User { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int Score { get; set; }

        //עוד לא עשיתי עליהם מיגריישין
        public int Xp { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    }
}
