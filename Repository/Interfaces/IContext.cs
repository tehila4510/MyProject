using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Interfaces
{
    public interface IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSkillProgress> UserSkillProgress { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public Task<int> Save();
    }
}
