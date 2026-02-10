using AutoMapper;
using Common;
using Common.Dto.Sessions;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class SessionService : IService<SessionDto>
    {
        private readonly IRepository<Session> repository;
        private readonly IMapper mapper;

        public SessionService(IRepository<Session> repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;

        }
        public async Task<SessionDto> Add(SessionDto item)
        {
            var session = mapper.Map<Session>(item);

            session.StartedAt = DateTime.UtcNow;

            var s = await repository.AddItem(session);

            return mapper.Map<SessionDto>(s);
        }

        public async Task Delete(int id)
        {   var session = await repository.GetById(id);
            if(session == null)
                throw new KeyNotFoundException($"Session with id {id} not found");

            await repository.DeleteItem(id);
        }

        public async Task<List<SessionDto>> GetAll()
        {
            var sessions = await repository.GetAll();
            if(sessions == null || sessions.Count == 0)
                throw new NotFoundException("No sessions found");
            return mapper.Map<List<SessionDto>>(sessions);
        }

        public async Task<SessionDto> GetById(int id)
        {
            var session =await repository.GetById(id);
            if(session == null)
                throw new KeyNotFoundException($"Session with id {id} not found");
            return mapper.Map<SessionDto>(session);
        }

        public async Task<SessionDto> Update(int id, SessionDto item)
        {
            var existingSession = await repository.GetById(id);
            if(existingSession == null)
                throw new KeyNotFoundException($"Session with id {id} not found");
            var session =await repository.UpdateItem(id, mapper.Map<Session>(item));
           // session.EndedAt = DateTime.UtcNow;
            return mapper.Map<SessionDto>(session);
        }
    }
}
