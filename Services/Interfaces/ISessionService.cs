using Common.Dto.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISessionService
    {
        Task<List<SessionDto>> GetByUser(int userId);

    }
}
