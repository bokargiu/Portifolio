using Portifolio.Server.Enums;

namespace Portifolio.Server.DTOs.Projects
{
    public class TemplateProject
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public ProjectType? Type { get; set; } = null;
        public DateOnly? Start { get; set; } = null;
        public DateOnly? End { get; set; } = null;
        public List<string> IconsTech { get; set; } = new List<string>();
    }
}
