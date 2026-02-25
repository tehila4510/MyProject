using Common.Dto.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> Register(UserUpdateDto registerDto);
        Task<string> Login(LoginDto loginDto); // מחזיר JWT
        Task<UserDto> GetById(int id);
        //Task<List<UserDto>> GetAll();
        Task<UserDto> Update(int id, UserUpdateDto updateDto);
        Task Delete(int id);
        Task<bool> UserExists(string email);
    }
}
