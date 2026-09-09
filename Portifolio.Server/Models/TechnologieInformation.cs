using Microsoft.EntityFrameworkCore;

namespace Portifolio.Server.Models
{
    public class TechnologieInformation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = String.Empty;
        public string IconName { get; set; } = String.Empty;
        public string Infos { get; set; } = String.Empty;
        public int Position { get; set; } = 0;
    }
}
