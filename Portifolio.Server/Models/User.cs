using Portifolio.Server.Enums;

namespace Portifolio.Server.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public TypeUser Type { get; set; } = TypeUser.User;

        public User() {}
        public User(bool admin = false)
        {
            Type = admin ? TypeUser.Admin : TypeUser.User;
        }
    }
}
