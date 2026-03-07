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
    public class UserSkillProgressRepository : IProgressRepository
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
       
        public async Task<UserSkillProgress> GetById(int userId,int skillId)
        {
            return await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserId == userId && x.SkillId==skillId);
        }
        public async Task<UserSkillProgress> UpdateItem(int userId, int skillId, UserSkillProgress item)
        {
            var usp = await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserId == userId && x.SkillId == skillId);
            if (usp != null)
            {
                usp.Mastery = item.Mastery;
                usp.LastPracticed = item.LastPracticed;
            }
            return usp;
        }

      
        public async Task DeleteItem(int userId, int skillId)
        {
            var usp = await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserId == userId && x.SkillId==skillId);
            if (usp != null)
            {
                ctx.UserSkillProgress.Remove(usp);
                await ctx.Save();
            }
        }
    }
}
