using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace DataContext.Entities
{
    public class User
    {
        public int UserId { get; set; } // PK

        // ===== פרטים אישיים =====
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ===== מצב לימוד כללי =====

        //Levels.AllLevels של רמה מתוך ID מחזיק
        public int CurrentLevel { get; set; }         // רמת שפה כללית לפי המבחן התאמה
        public int Xp { get; set; } = 0;              // נקודות צבירה
        public int Streak { get; set; } = 0;          // רצף ימים
        public int Hearts { get; set; } = 5;          // מספר פסילות אפשריות- לרענן כל 24 שעות
        public DateTime? LastActivity { get; set; }   // פעילות אחרונה


        // PRO אם אני מוסיפה בהמשך אפשרות של 
        public bool IsPro { get; set; }               // חינמי / פרו
        public DateTime? ProUntil { get; set; }       // PRO תוקף 



        public ICollection<UserSkillProgress> SkillsProgress { get; set; } =  new List<UserSkillProgress>();
        public ICollection<UserAnswer> UserAnswers { get; set; }= new List<UserAnswer>();
        public ICollection<Session> Sessions { get; set; }=new List<Session>();
    }

}
