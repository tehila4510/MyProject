using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto.UserProgress
{
    public class UserSkillProgressDto
    {
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public int Mastery { get; set; }
        public DateTime? LastPracticed { get; set; }
    }
}
