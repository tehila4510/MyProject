using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Entities
{
    public class Question
    {

        public int QuestionId { get; set; } // PK
        public int SkillId { get; set; }
        public int LevelId { get; set; }
        public string QuestionText { get; set; }

        // Bitmask במקום מערך ENUM
        public int QuestionTypeMask { get; set; }

        public AudioTypeEnum AudioType { get; set; }
        public string? AudioSource { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

        // פונקציות עזר לעבודה עם Bitmask
        public void SetQuestionTypes(params QuestionTypeEnum[] types)
        {
            QuestionTypeMask = types.Aggregate(0, (mask, type) => mask | (int)type);
        }

        public QuestionTypeEnum[] GetQuestionTypes()
        {
            return Enum.GetValues<QuestionTypeEnum>()
                       .Where(t => (QuestionTypeMask & (int)t) != 0)
                       .ToArray();
        }

        public bool HasQuestionType(QuestionTypeEnum type)
        {
            return (QuestionTypeMask & (int)type) != 0;
        }
    }
}
