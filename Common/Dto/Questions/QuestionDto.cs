using Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Dto.Question
{
    public class QuestionDto
    {

        public int QuestionId { get; set; }

            [Required(ErrorMessage = "This field is required")]
        public int SkillId { get; set; }

            [Required(ErrorMessage = "This field is required")]
        public int LevelId { get; set; }
        //title
            [Required(ErrorMessage = "This field is required")]
            [StringLength(200, MinimumLength = 5, ErrorMessage = "Length must be between 5 and 200 characters")]
        public string QuestionText { get; set; }
        public string? AudioSource { get; set; }
        public byte[]? ImageUrl { get; set; }
        public IFormFile? file { get; set; }
        public ICollection<QuestionOptionDto>? Options { get; set; } = new List<QuestionOptionDto>();

    }
}
