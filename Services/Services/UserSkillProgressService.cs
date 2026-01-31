using Common.Dto;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class UserSkillProgressService : IService<UserSkillProgressDto>
    {
        public Task<UserSkillProgressDto> Add(UserSkillProgressDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserSkillProgressDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserSkillProgressDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserSkillProgressDto> Update(int id, UserSkillProgressDto item)
        {
            throw new NotImplementedException();
        }
    }
}
