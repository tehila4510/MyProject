using Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Dto.User
{
    public class UserDto
    {
        //תצוגה
        public int UserId { get; set; } // PK     
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public byte[]? AvatarUrl { get; set; }
        public IFormFile? file { get; set; }
        public int CurrentLevel { get; set; }
        public int Xp { get; set; } = 0;
        public int Streak { get; set; } = 0;
        public int Hearts { get; set; } = 5;
        public DateTime? LastActivity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProUntil { get; set; }
    }
}
