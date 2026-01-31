using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Dto;

namespace Services.Services
{
    public class QuestionOptionService : IService<QusetionOptionDto>
    {
        public Task<QusetionOptionDto> Add(QusetionOptionDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<QusetionOptionDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<QusetionOptionDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QusetionOptionDto> Update(int id, QusetionOptionDto item)
        {
            throw new NotImplementedException();
        }
    }
}
