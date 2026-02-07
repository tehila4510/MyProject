using Repository.Entities;
using Microsoft.EntityFrameworkCore;
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
            ctx.UserSkillProgress.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int UserId, int skillID)
        {
           var usp=await ctx.UserSkillProgress.FirstOrDefaultAsync(x=> x.UserId == UserId && x.SkillId==skillID);
            if (usp != null)
            {
                ctx.UserSkillProgress.Remove(usp);
                await ctx.Save();
            }
        }

        public async Task<List<UserSkillProgress>> GetAll()
        {
            return await ctx.UserSkillProgress.ToListAsync();
        }

        public async Task<UserSkillProgress> GetById(int UserId, int skillID)
        {
            return await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserId == UserId && x.SkillId == skillID);
        }
    
        public async Task<UserSkillProgress> UpdateItem(int UserId, int skillID, UserSkillProgress item)
        {
            var usp = await ctx.UserSkillProgress.FirstOrDefaultAsync(x => x.UserId == UserId && x.SkillId == skillID);
            if (usp != null)
            {
                usp.Mastery = item.Mastery;
                usp.LastPracticed = item.LastPracticed;
            }
            return usp;
        }
        public Task DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
        public Task<UserSkillProgress> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public Task<UserSkillProgress> UpdateItem(int id, UserSkillProgress item)
        {
            throw new NotImplementedException();
        }
    }
}
