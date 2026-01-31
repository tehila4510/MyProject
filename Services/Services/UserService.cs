using Common.Dto;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class UserService : IService<UserDto>, IsExist<UserDto>
    {
        public Task<UserDto> Add(UserDto item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public UserDto Exist(Login l)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> Update(int id, UserDto item)
        {
            throw new NotImplementedException();
        }
    }
}
