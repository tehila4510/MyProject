using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public int SkillId { get; set; }
        public int LevelId { get; set; }
        public string QuestionText { get; set; }

        public QuestionTypeEnum[] QuestionTypes { get; set; }
        public AudioTypeEnum AudioType { get; set; }
        public string AudioSource { get; set; }
        public string ImageURL { get; set; }

        public ICollection<QuestionOptionDto> Options { get; set; } = new List<QuestionOptionDto>();

    }
}
