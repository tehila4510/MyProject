using Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto.Question
{
    public class QuestionDto
    {

        public int QuestionId { get; set; }
        public int SkillId { get; set; }
        public int LevelId { get; set; }
        //title
        public string QuestionText { get; set; }
        public string? AudioSource { get; set; }
        public byte[]? ImageUrl { get; set; }
        public IFormFile? file { get; set; }
        public ICollection<QuestionOptionDto>? Options { get; set; } = new List<QuestionOptionDto>();

    }
}
