using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Repository.Repositories
{
    public class QestionRepository : IRepository<Question>
    {
        private readonly IContext ctx;
        public QestionRepository(IContext context)
        {
            ctx = context;
        }
        public async Task<Question> AddItem(Question item)
        {
            await ctx.Questions.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var q =await ctx.Questions.FirstOrDefaultAsync(x => x.QuestionId == id);
            if (q!=null)
            {
                ctx.Questions.Remove(q);
            }
            await ctx.Save();
        }

        public Task<List<Question>> GetAll()
        {
            return ctx.Questions.ToListAsync();
        }

        public async Task<Question> GetById(int id)
        {
            return await ctx.Questions.FirstOrDefaultAsync(x => x.QuestionId == id);
        }

        public async Task<Question> UpdateItem(int id, Question item)
        {
            var q = await ctx.Questions.FirstOrDefaultAsync(x => x.QuestionId == id);
            if (q != null)
            {
                q.SkillId = item.SkillId;
                q.LevelId = item.LevelId;
                q.QuestionText = item.QuestionText;
                q.QuestionTypeMask = item.QuestionTypeMask;
                q.AudioType = item.AudioType;
                q.AudioSource = item.AudioSource;
                q.ImageURL = item.ImageURL;
                await ctx.Save();
                return q;
            }
            else
            {
                return null;
            }
        }
    }
}
