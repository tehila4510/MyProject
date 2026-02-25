using AutoMapper;
using Common;
using Common.Dto.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Repository.Entities;
using Repository.Interfaces;

using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Services.Services
{
    public class UserService :IUserService
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public UserService(IRepository<User> repository, IMapper mapper, IConfiguration configuration)
        {
                this.repository = repository;
                this.mapper = mapper;
                this.configuration = configuration;
        }
        public async Task<bool> UserExists(string email)
        {
            var users = await repository.GetAll();
            return users.Any(u => u.Email == email);
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

        public async Task<string> Login(LoginDto loginDto)
        {
            var users = await repository.GetAll();
            var user = users.FirstOrDefault(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return GenerateToken(mapper.Map<UserDto>(user));
        }

        public async Task<UserDto> Register(UserUpdateDto registerDto)
        {
            if (await UserExists(registerDto.Email))
                throw new InvalidOperationException($"User with email {registerDto.Email} already exists");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.PasswordHash);
            registerDto.PasswordHash = hashedPassword;

            // Save avatar file
            if (registerDto.file != null)
            {
                var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(registerDto.file.FileName);
                var path = System.IO.Path.Combine(Environment.CurrentDirectory, "ProfileImages", fileName);
                using var fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                await registerDto.file.CopyToAsync(fs);
                registerDto.AvatarUrl = Encoding.UTF8.GetBytes(fileName);
            }
            var user = mapper.Map<User>(registerDto);
            var createdUser = await repository.AddItem(user);

            return mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto> Update(int id, UserUpdateDto updateDto)
        {
            var existingUser = await repository.GetById(id);
            if (existingUser == null) throw new KeyNotFoundException();

            if (!string.IsNullOrEmpty(updateDto.PasswordHash))
            {
                updateDto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.PasswordHash);
            }

            // Update avatar if provided
            if (updateDto.file != null)
            {
                var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(updateDto.file.FileName);
                var path = System.IO.Path.Combine(Environment.CurrentDirectory, "ProfileImages", fileName);
                using var fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                await updateDto.file.CopyToAsync(fs);
                updateDto.AvatarUrl = Encoding.UTF8.GetBytes(fileName);
            }

            var updatedUser = await repository.UpdateItem(id, mapper.Map<User>(updateDto));
            return mapper.Map<UserDto>(updatedUser);
        }
        public async Task Delete(int id)
        {
            var user = await repository.GetById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with id {id} not found");
            await repository.DeleteItem(id);
        }

        private string GenerateToken(UserDto u)
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, u.UserId.ToString()),
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Name, u.Name)
            };
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
