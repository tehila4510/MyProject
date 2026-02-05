using Common.Dto.UserProgress;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class UserAnswerService : IService<UserAnswerDto>
    {
        public Task<UserAnswerDto> Add(UserAnswerDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserAnswerDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserAnswerDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserAnswerDto> Update(int id, UserAnswerDto item)
        {
            throw new NotImplementedException();
        }
    }
}
