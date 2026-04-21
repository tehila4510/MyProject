using AutoMapper;
using Common;
using Common.Dto.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Repository.Entities;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService :IUserService
    {
        private readonly IRepository<User> repository;
        private readonly ITokenService tokenService;
        private readonly IFileService fileService;
        private readonly IMapper mapper;

        public UserService(IRepository<User> repository, IMapper mapper, ITokenService tokenService, IFileService fileService)
        {
                this.repository = repository;
                this.tokenService = tokenService;
                this.fileService = fileService;
                this.mapper = mapper;
        }
        public async Task<bool> UserExists(string email)
        {
            var exists = await repository
            .GetByCondition(u => u.Email == email)
             .AnyAsync();
            return exists;
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

        public async Task<object> Login(LoginDto loginDto)
        {
            var user = await repository
            .GetByCondition(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
    
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var userDto = mapper.Map<UserDto>(user);
            string token = tokenService.GenerateToken(userDto);
            return new { User = userDto, Token = token };
        }

        public async Task<object> Register(UserUpdateDto registerDto)
        {

            if (await UserExists(registerDto.Email))
                throw new InvalidOperationException($"User with email {registerDto.Email} already exists");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            registerDto.Password = hashedPassword;

            // Save avatar file
            if (registerDto.file != null)
            {
                var fileName = await fileService.SaveFileAsync(registerDto.file);
                registerDto.AvatarUrl = fileName;
            }
            var user = mapper.Map<User>(registerDto);
            var createdUser = await repository.AddItem(user);

            var userDto= mapper.Map<UserDto>(createdUser);
            var token= tokenService.GenerateToken(userDto);
            return new { User = userDto, Token = token };
        }

        public async Task<UserDto> Update(int id, UserUpdateDto updateDto)
        {
            var user = await repository.GetById(id);

            if (!string.IsNullOrEmpty(updateDto.Name))
                user.Name = updateDto.Name;

            if (updateDto.file != null)
            {
                var fileName = await fileService.SaveFileAsync(updateDto.file);
                user.AvatarUrl = fileName;
            }

            await repository.UpdateItem(id, user);
            return mapper.Map<UserDto>(user);
        }
        public async Task Delete(int id)
        {
            var user = await repository.GetById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with id {id} not found");
            await repository.DeleteItem(id);
        }
    }
}
