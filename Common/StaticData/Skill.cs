using System;
using System.Collections.Generic;
using System.Text;

namespace Common.StaticData
{
    public class SkillInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RecommendedLevelId { get; set; } // מתייחס ל-Level ID
    }

    public static class Skill
    {
        public static readonly Dictionary<int, SkillInfo> AllSkills = new()
        {
            { 1, new SkillInfo { Name = "Vocabulary", Description = "Words and phrases", RecommendedLevelId = 1 } },
            { 2, new SkillInfo { Name = "Grammar", Description = "Grammar rules", RecommendedLevelId = 2 } },
            { 3, new SkillInfo { Name = "Verbs", Description = "Verb conjugation", RecommendedLevelId = 1 } },
            { 4, new SkillInfo { Name = "Listening", Description = "Comprehension skills", RecommendedLevelId = 2 } },
            { 5, new SkillInfo { Name = "Reading", Description = "Reading comprehension", RecommendedLevelId = 2 } },
            { 6, new SkillInfo { Name = "Writing", Description = "Writing practice", RecommendedLevelId = 3 } },
            { 7, new SkillInfo { Name = "Pronunciation", Description = "Speaking and pronunciation", RecommendedLevelId = 1 } },
            { 8, new SkillInfo { Name = "Phrases", Description = "Useful phrases for conversation", RecommendedLevelId = 1 } }
           //להוסיף אפשרות תשע של שיחה עם בוט
        };
    }
}
