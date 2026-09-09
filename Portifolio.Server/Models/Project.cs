using Portifolio.Server.Enums;
using Portifolio.Server.DTOs.Projects;

namespace Portifolio.Server.Models
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public ProjectType Type { get; set; } = ProjectType.Em_Andamento;
        public DateOnly Start { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? End { get; set; } = null;
        public List<string> IconsTech { get; set; } = new List<string>();

        public Project() { }
        public Project(TemplateProject dto)
        {
            Name = dto.Name;
            Description = dto.Description;
            Url = dto.Url ?? string.Empty;
            Type = dto.Type ?? ProjectType.Em_Andamento;
            Start = dto.Start ?? DateOnly.FromDateTime(DateTime.Now);
            End = dto.End;
            IconsTech = dto.IconsTech;
        }
    }
}
