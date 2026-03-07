using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IProgressRepository
    {
        Task<List<UserSkillProgress>> GetAll();
        Task<UserSkillProgress> GetById(int userId,int skillId);
        Task<UserSkillProgress> AddItem(UserSkillProgress item);
        Task<UserSkillProgress> UpdateItem(int userId, int skillId, UserSkillProgress item);
        Task DeleteItem(int userId, int skillId);
    }
}
