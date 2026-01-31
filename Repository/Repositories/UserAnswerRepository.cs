using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DataContext.Entities;


namespace Repository.Repositories
{
    public class UserAnswerRepository : IRepository<UserAnswer>
    {
        private readonly IContext ctx;
        public UserAnswerRepository(IContext context)
        {
            ctx = context;
        }
        public async Task<UserAnswer> AddItem(UserAnswer item)
        {
            ctx.UserAnswers.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var ua =await ctx.UserAnswers.FirstOrDefaultAsync(x => x.AnswerId == id);
            if (ua != null)
            {
                ctx.UserAnswers.Remove(ua);
                await ctx.Save();
            }
        }

        public async Task<List<UserAnswer>> GetAll()
        {
            return await ctx.UserAnswers.ToListAsync();
        }

        public async Task<UserAnswer> GetById(int id)
        {
            return await ctx.UserAnswers.FirstOrDefaultAsync(x => x.AnswerId == id);
        }

        public async Task<UserAnswer> UpdateItem(int id, UserAnswer item)
        {
            var ua = await ctx.UserAnswers.FirstOrDefaultAsync(x => x.AnswerId == id);
            if (ua != null)
            {
                ua.UserId = item.UserId;
                ua.QuestionId = item.QuestionId;
                ua.UserAnswerText = item.UserAnswerText;
                ua.IsCorrect = item.IsCorrect;
                ua.AnsweredAt = item.AnsweredAt;
                ua.TimeToAnswerMs = item.TimeToAnswerMs;
                ctx.UserAnswers.Update(ua);
                await ctx.Save();
            }
            return ua;
        }
    }
}
