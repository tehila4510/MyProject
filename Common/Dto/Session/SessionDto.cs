using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto.Sessions
{
    public class SessionDto
    {
        public int SessionId { get; set; } // PK
        public int UserId { get; set; } // FK
        public int Score { get; set; }

        public int Xp { get; set; }
        //הוא השיג XP האם אמור להיות לי פה כמה 
        public double DurationInMinutes { get; set; }
    }
}
