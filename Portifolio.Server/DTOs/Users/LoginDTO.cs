namespace Portifolio.Server.DTOs.Users
{
    public class LoginDTO
    {
        public string NameOrEmail { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
