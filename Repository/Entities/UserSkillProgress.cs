using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Entities
{
    public class UserSkillProgress
    {
        public int UserSkillProgressId { get; set; }
        public int UserId { get; set; } // FK
        public User User { get; set; }
        public int SkillId { get; set; } // FK
        public int Mastery { get; set; } // 0–100
        public DateTime? LastPracticed { get; set; }
    }
}
