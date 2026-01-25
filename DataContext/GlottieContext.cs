using System;
using DataContext.Entities;
using DataContext.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataContext
{
    public class GlottieContext : DbContext
    {
        public GlottieContext() { }

        public GlottieContext(DbContextOptions<GlottieContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserSkillProgress> UserSkillProgress { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSkillProgress>()
                .HasKey(usp => new { usp.UserId, usp.SkillId });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-2TSTMUL;Database=GlottieDB;Trusted_Connection=True;TrustServerCertificate=True"
            );
        }
    }
}