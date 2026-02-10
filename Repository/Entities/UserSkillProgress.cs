using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Entities
{
    public class UserSkillProgress
    {
        //public int userSkillProgressId { get; set; }
        public int UserId { get; set; } // FK + PK
        public User User { get; set; }
        public int SkillId { get; set; } // FK + PK
        public int Mastery { get; set; } // 0–100
        public DateTime? LastPracticed { get; set; }
    }
}
