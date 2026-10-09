using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.Users;
using Portifolio.Server.Enums;
using Portifolio.Server.Models;
using Portifolio.Server.Services.AuthServices;
using System.ComponentModel.DataAnnotations;

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
            if (dto == null || !ValidPassword(dto.Password))
                return new BaseResponse<User>(400, message: "Invalid user data.");

            if (new EmailAddressAttribute().IsValid(dto.Name.Trim()))
                return new BaseResponse<User>(400, message: "Name cannot be an email");

            if (!new EmailAddressAttribute().IsValid(dto.Email.Trim()))
                return new BaseResponse<User>(400, message: "This Email is not Valid");

            var existingUser = await _context.Users.Where(u => u.Email.Equals(dto.Email.Trim()) || u.Name.Equals(dto.Name.Trim())).AnyAsync();

            if (existingUser)
                return new BaseResponse<User>(400, message: "A user with this email or name already exists.");

            var user = new User(true);
            user.Email = dto.Email.Trim();
            user.Name = dto.Name.Trim();
            user.Type = TypeUser.User;
            user.Password = Argon2.Hash(dto.Password, timeCost: 5);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new BaseResponse<User>(201, data: user);
        }

        public async Task<BaseResponse<User>> Login(LoginDTO dto)
        {
            if (dto is null)
                return new BaseResponse<User>(400);
            var login = dto.NameOrEmail.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(dto.Password))
                return new BaseResponse<User>(400, message: "Email and password cannot be empty.");
            var user = login.Contains('@')
                ? (!new EmailAddressAttribute().IsValid(login)) 
                       ? null : await _context.Users.FirstOrDefaultAsync(u => u.Email == login)
                : await _context.Users.FirstOrDefaultAsync(u => u.Name == login);

            if (user == null)
                return new BaseResponse<User>(404, message: "User not found.");

            if(!Argon2.Verify(user.Password, dto.Password))
                return new BaseResponse<User>(403, message: "Invalid password.");

            return new BaseResponse<User>(200, data: user);
        }

        public bool ValidPassword(string password)
        {
            if (password.Count() < 8)
                return false;

            int num = 0;
            if (password.Any(char.IsUpper))
                num++;

            if (password.Any(char.IsLower))
                num++;

            if (password.Any(char.IsDigit))
                num++;

            if (password.Any(c => char.IsPunctuation(c) || char.IsSymbol(c)))
                num++;

            return num >= 3;
        }
    }
}
