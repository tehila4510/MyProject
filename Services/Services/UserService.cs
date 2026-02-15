using AutoMapper;
using Common;
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
        public async Task<UserDto> Add(UserUpdateDto item)
        {
            var user = await repository.AddItem(mapper.Map<User>(item));
            return mapper.Map<UserDto>(user);
        }

        public Task<UserDto> Add(UserDto item)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            var user = await repository.GetById(id);
            if(user == null)
                throw new KeyNotFoundException($"User with id {id} not found");
            await repository.DeleteItem(id);
        }

        public async Task<UserDto> Exist(LoginDto l)
        {
            var users = await GetAll();
            var user= users.FirstOrDefault(u => u.Email == l.Email && u.PasswordHash == l.Password);
            return user!=null? user : null;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await repository.GetAll();
            if(users == null || users.Count == 0)
                throw new NotFoundException("No users found");
            return mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await repository.GetById(id);
            if(user == null)
                throw new KeyNotFoundException($"User with id {id} not found");
            return mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Update(int id, UserUpdateDto item)
        {
            var existingUser = await repository.GetById(id);
            if(existingUser == null)
                throw new KeyNotFoundException($"User with id {id} not found");
            var user=await repository.UpdateItem(id, mapper.Map<User>(item));
            return mapper.Map<UserDto>(user);
        }

        public Task<UserDto> Update(int id, UserDto item)
        {
            throw new NotImplementedException();
        }
    }
}
