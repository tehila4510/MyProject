using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    public class QuestionOptionRepository : IRepository<QuestionOption>
    {
        private readonly IContext ctx;
        public QuestionOptionRepository(IContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<QuestionOption> AddItem(QuestionOption item)
        {
            await ctx.QuestionOptions.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var qo=await ctx.QuestionOptions.FirstOrDefaultAsync(x=> x.OptionId==id);
            if (qo != null) { 
                ctx.QuestionOptions.Remove(qo);
                await ctx.Save();
            }
        }

        public async Task<List<QuestionOption>> GetAll()
        {
            return await ctx.QuestionOptions.ToListAsync();
        }

        public async Task<QuestionOption> GetById(int id)
        {
            return await ctx.QuestionOptions.FirstOrDefaultAsync(x => x.OptionId == id);
        }

        public async Task<QuestionOption> UpdateItem(int id, QuestionOption item)
        {
            var qo = await ctx.QuestionOptions.FirstOrDefaultAsync(x => x.OptionId == id);
            if (qo != null) { 
                qo.QuestionId=item.QuestionId;
                qo.IsCorrect=item.IsCorrect;
                qo.OptionText=item.OptionText;
                await ctx.Save();
            }
            return qo;
        }
    }
}
