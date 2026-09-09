using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.Users;
using Portifolio.Server.Models;
using Portifolio.Server.Services.AuthServices;

namespace Portifolio.Server.Services.User_Services
{
    public class UserService : IUserService
    {
        private readonly IAuthService _auth;
        private readonly Database.DB _context;
        public UserService(IAuthService auth, Database.DB context)
        {
            _auth = auth;
            _context = context;
        }

        public async Task<BaseResponse<User>> CreateUser(TemplateUser dto)
        {
            if (dto == null)
                return new BaseResponse<User>(400, message: "Invalid user data.");
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return new BaseResponse<User>(400, message: "A user with this email already exists.");
            var user = new User(true);
            user.Email = dto.Email;
            user.Name = dto.Name;
            user.Password = Argon2.Hash(dto.Password, timeCost: 5);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new BaseResponse<User>(201, data: user);
        }

        public async Task<BaseResponse<User>> Login(LoginDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return new BaseResponse<User>(400, message: "Email and password cannot be empty.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return new BaseResponse<User>(404, message: "User not found.");

            if(!Argon2.Verify(user.Password, dto.Password))
                return new BaseResponse<User>(403, message: "Invalid password.");

            return new BaseResponse<User>(200, data: user);
        }
    }
}
