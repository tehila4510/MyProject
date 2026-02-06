using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Dto.User
{
    public class UserDto
    {
        public int UserId { get; set; } // PK

        public string Name { get; set; }
        public string Email { get; set; }
        public  byte[]? AvatarUrl { get; set; }
        // Password is stored as a hash for security reasons
        public IFormFile? file { get; set; }
        public int CurrentLevel { get; set; }       
        public int Xp { get; set; } = 0;            
        public int Streak { get; set; } = 0;        
        public int Hearts { get; set; } = 5;        
        public DateTime? LastActivity { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsPro { get; set; }             
        public DateTime? ProUntil { get; set; }     
    }
}
