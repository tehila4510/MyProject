using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IContext ctx;
        public UserRepository(IContext context)
        {
            ctx = context;
        }
        public async Task<User> AddItem(User item)
        {
            ctx.Users.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var u = await ctx.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (u != null)
            {
                ctx.Users.Remove(u); 
            }
            await ctx.Save();
        }

        public async Task<List<User>> GetAll()
        {
            return await ctx.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await ctx.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<User> UpdateItem(int id, User item)
        {
            var u = await ctx.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (u != null)
            {
                u.Name = item.Name;
                u.Email = item.Email;
                u.AvatarUrl = item.AvatarUrl;
                u.PasswordHash = item.PasswordHash;
                u.CurrentLevel = item.CurrentLevel;
                u.Xp = item.Xp;
                u.Streak = item.Streak;
                u.Hearts = item.Hearts;
                u.LastActivity = item.LastActivity;
                u.IsPro = item.IsPro;
                u.ProUntil = item.ProUntil;
                
                await ctx.Save();      
            }
            return u;
        }
    }
}
