using Microsoft.IdentityModel.Tokens;
using Portifolio.Server.Database;
using Portifolio.Server.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Portifolio.Server.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly Database.DB _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IConfiguration configuration, Database.DB context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(User user)
        {
            var settings = _configuration.GetSection("Jwt");

            var tokenOP = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Type.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = settings["Issuer"],
                Audience = settings["Audience"],
                SigningCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(
                                            Encoding.UTF8.GetBytes(settings["Key"])),
                                            SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenOP);
            return tokenHandler.WriteToken(token);
        }
    }
}
