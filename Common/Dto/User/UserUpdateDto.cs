using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Common.Dto.User
{
    public class UserUpdateDto
    {
        //הוספה ועדכון
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "This field is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Length must be between 2 and 50 characters")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "This field is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        //    ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string? PasswordHash { get; set; }

        public int? CurrentLevel { get; set; }

        public byte[]? AvatarUrl { get; set; }
        public IFormFile? file { get; set; }
    }
}
