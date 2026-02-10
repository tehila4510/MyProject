using Common.StaticData;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    // לא ביצעתי הורשה מהממשק הרפוזיטורי כי יש פה 2 מפתחות ראשיים
    public class UserSkillProgressRepository :IRepository<UserSkillProgress>
    {
        private readonly IContext ctx;
        public UserSkillProgressRepository(IContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<UserSkillProgress> AddItem(UserSkillProgress item)
        {
            await ctx.UserSkillProgress.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task<List<UserSkillProgress>> GetAll()
        {
            return await ctx.UserSkillProgress.ToListAsync();
        }

        public async Task DeleteItem(int id)
        {
            var usp = await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserSkillProgressId==id);
            if (usp != null)
            {
                ctx.UserSkillProgress.Remove(usp);
                await ctx.Save();
            }
        }
        public async Task<UserSkillProgress> GetById(int id)
        {
            return await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserSkillProgressId == id);
        }
        public async Task<UserSkillProgress> UpdateItem(int id, UserSkillProgress item)
        {
            var usp = await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserSkillProgressId == id);
            if (usp != null)
            {
                usp.Mastery = item.Mastery;
                usp.LastPracticed = item.LastPracticed;
            }
            return usp;
        }
    }
}
