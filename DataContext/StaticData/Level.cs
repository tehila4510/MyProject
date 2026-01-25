using System;
using System.Collections.Generic;
using System.Text;

namespace DataContext.StaticData
{
    public class LevelInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinXp { get; set; }
    }

    public static class Level
    {
        public static readonly Dictionary<int, LevelInfo> AllLevels = new()
        {
            { 1, new LevelInfo { Name = "A1", Description = "Beginner", MinXp = 0 } },
            { 2, new LevelInfo { Name = "A2", Description = "Elementary", MinXp = 100 } },
            { 3, new LevelInfo { Name = "B1", Description = "Intermediate", MinXp = 250 } },
            { 4, new LevelInfo { Name = "B2", Description = "Upper-Intermediate", MinXp = 500 } },
            { 5, new LevelInfo { Name = "C1", Description = "Advanced", MinXp = 1000 } },
            { 6, new LevelInfo { Name = "C2", Description = "Mastery", MinXp = 1500 } }  
        };
    }
}
