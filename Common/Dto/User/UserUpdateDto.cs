using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Dto.User
{
    public class UserUpdateDto
    {
        //הוספה ועדכון

        [Required(ErrorMessage = "This field is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Length must be between 2 and 50 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public byte[]? AvatarUrl { get; set; }
        public IFormFile? file { get; set; }
    }
}
