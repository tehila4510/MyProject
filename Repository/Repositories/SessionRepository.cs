using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    public class SessionRepository : IRepository<Session>
    {
        private readonly IContext ctx;
        public SessionRepository(IContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<Session> AddItem(Session item)
        {
            await ctx.Sessions.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var s = await ctx.Sessions.FirstOrDefaultAsync(x => x.SessionId == id);
            if (s != null)
            {
                ctx.Sessions.Remove(s);
                await ctx.Save();
            }
        }

        public async Task<List<Session>> GetAll()
        {
           return await ctx.Sessions.ToListAsync();
        }

        public async Task<Session> GetById(int id)
        {
           return await ctx.Sessions.FirstOrDefaultAsync(x => x.SessionId == id);
        }

        public async Task<Session> UpdateItem(int id, Session item)
        {
            var s = await ctx.Sessions.FirstOrDefaultAsync(x => x.SessionId == id);
            if (s != null)
            {
                s.StartedAt=item.StartedAt;
                s.EndedAt=item.EndedAt;
                s.Score=item.Score;
                s.UserId=item.UserId;
                await ctx.Save();
            }
            return s;
        }
    }
}
