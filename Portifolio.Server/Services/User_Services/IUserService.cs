using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.Users;
using Portifolio.Server.Models;

namespace Portifolio.Server.Services.User_Services
{
    public interface IUserService
    {
        public Task<BaseResponse<User>> CreateUser(TemplateUser dto);
        public Task<BaseResponse<User>> Login(LoginDTO dto);
    }
}
