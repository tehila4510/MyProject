using AutoMapper;
using Common.Dto.User;
using Repository.Entities;
using Repository.Interfaces;

using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class UserService : IService<UserDto>, IsExist<UserDto>
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        public UserService(IRepository<User> repository, IMapper mapper)
        {
                this.repository = repository;
                this.mapper = mapper;
        }
        public async Task<UserDto> Add(UserDto item)
        {
            var user = await repository.AddItem(mapper.Map<User>(item));
            return mapper.Map<UserDto>(user);
        }

        public async Task Delete(int id)
        {
            await repository.DeleteItem(id);
        }

        public UserDto Exist(LoginDto l)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await repository.GetAll();
            return mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await repository.GetById(id);
            return mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Update(int id, UserDto item)
        {
            var user=await repository.UpdateItem(id, mapper.Map<User>(item));
            return mapper.Map<UserDto>(user);
        }
    }
}
