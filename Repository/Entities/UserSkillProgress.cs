using System;
using System.Collections.Generic;
using System.Text;

namespace DataContext.Entities
{
    public class UserSkillProgress
    {
        public int UserId { get; set; } // FK + PK
        public User User { get; set; }
        
        //Skills.AllSkills של מיומנות מתוך ID מחזיק
        public int SkillId { get; set; } // FK + PK
        public int Mastery { get; set; } // 0–100
        public DateTime? LastPracticed { get; set; }
    }
}
