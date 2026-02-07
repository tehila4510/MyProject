using System;
using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace DataContext.model

{
    public class GlottieContext : DbContext,IContext
    {
        private readonly string? _connection;
        public GlottieContext(string _connection) {
            this._connection = _connection;
        }

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

            // הקשר בין Session ל-UserAnswer
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Session)
                .WithMany(s => s.UserAnswers)
                .HasForeignKey(ua => ua.SessionId)
                .OnDelete(DeleteBehavior.Restrict); // או NoAction

            base.OnModelCreating(modelBuilder);
        }
        public Task Save()
        {
            return SaveChangesAsync();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_connection);
        }
    }
}