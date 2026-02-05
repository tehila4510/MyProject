using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto.Skills
{
    public class SkillDto
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RecommendedLevelId { get; set; }
    }
}
