using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class UserDto
    {
        public int UserId { get; set; } // PK

        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }


        public int CurrentLevel { get; set; }       
        public int Xp { get; set; } = 0;            
        public int Streak { get; set; } = 0;        
        public int Hearts { get; set; } = 5;        
        public DateTime? LastActivity { get; set; }
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o"); // ISO 8601

        public bool IsPro { get; set; }             
        public DateTime? ProUntil { get; set; }     
    }
}
