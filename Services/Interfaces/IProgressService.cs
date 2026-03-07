using Common.Dto.UserProgress;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProgressService
    {
        Task<List<UserSkillProgressDto>> GetAll();
        Task<UserSkillProgressDto> GetById(int userId, int skillId);
        Task<UserSkillProgressDto> Add(UserSkillProgressDto item);
        Task<UserSkillProgressDto> Update(int userId, int skillId, UserSkillProgressDto item);
        Task Delete(int userId, int skillId);
    }
}
