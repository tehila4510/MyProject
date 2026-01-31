using Common.Dto;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class QuestionService : IService<QuestionDto>
    {
        public Task<QuestionDto> Add(QuestionDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<QuestionDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<QuestionDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionDto> Update(int id, QuestionDto item)
        {
            throw new NotImplementedException();
        }
    }
}
