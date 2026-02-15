using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Dto.UserProgress
{
    public class UserSkillProgressDto
    {
        public int UserSkillProgressId { get; set; }
        [Required(ErrorMessage = "This field is required")]

        public int UserId { get; set; }
        [Required(ErrorMessage = "This field is required")]

        public int SkillId { get; set; }
        [Required(ErrorMessage = "This field is required")]

        public int Mastery { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        public DateTime? LastPracticed { get; set; }
    }
}
