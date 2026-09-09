using Portifolio.Server.Models;

namespace Portifolio.Server.Services.AuthServices
{
    public interface IAuthService
    {
        public string GenerateToken(User user);
    }
}
