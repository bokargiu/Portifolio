using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Server.DTOs.Users;
using Portifolio.Server.Services.AuthServices;
using Portifolio.Server.Services.User_Services;

namespace Portifolio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly IAuthService _auth;
        public UserController(IUserService user, IAuthService auth)
        {
            _user = user;
            _auth = auth;
        }

        [HttpPost]
        public async Task<IActionResult> Create(TemplateUser dto)
        {
            var created = await _user.CreateUser(dto);

            if(created.StatusCode == 201 && created.Data != null)
                return Created(created.Data.Id.ToString(),_auth.GenerateToken(created.Data));

            return created.StatusCode switch
            {
                400 => BadRequest(created.Message),
                _ => StatusCode(500, "An error occurred while creating the user.")
            };

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var response = await _user.Login(dto);

            return response.StatusCode switch
            {
                200 => Ok(_auth.GenerateToken(response.Data)),
                400 => BadRequest(response.Message),
                404 => NotFound(response.Message),
                403 => Forbid(response.Message),
                _ => StatusCode(500, "unexpected error.")
            };
        }
    }
}
