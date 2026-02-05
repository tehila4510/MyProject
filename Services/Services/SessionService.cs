using Common.Dto.Sessions;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class SessionService : IService<SessionDto>
    {
        public Task<SessionDto> Add(SessionDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SessionDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<SessionDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<SessionDto> Update(int id, SessionDto item)
        {
            throw new NotImplementedException();
        }
    }
}
