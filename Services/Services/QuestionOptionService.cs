using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Dto.Question;

namespace Services.Services
{
    public class QuestionOptionService : IService<QuestionOptionDto>
    {
        public Task<QuestionOptionDto> Add(QuestionOptionDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<QuestionOptionDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<QuestionOptionDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionOptionDto> Update(int id, QuestionOptionDto item)
        {
            throw new NotImplementedException();
        }
    }
}
