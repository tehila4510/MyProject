using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.UserProgress
{
    public class UserSkillProgressViewDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }

        public int ProgressPercent { get; set; }
        public int Accuracy { get; set; }

        public List<int> WeeklyXp { get; set; }

        public DateTime? LastPracticed { get; set; }
    }
}
