using Portifolio.Server.Models;

namespace Portifolio.Server.DTOs.TechInfos
{
    public class ResponseTechInfos
    {
        public string Id { get; } = String.Empty;
        public string Icon { get; } = String.Empty;
        public string Title { get; } = String.Empty;
        public string Infos { get; } = String.Empty;
        public bool Show { get; } = false;

        private ResponseTechInfos(TechnologieInformation entity)
        {
            Id = entity.Id.ToString();
            Icon = entity.IconName;
            Title = entity.Title;
            Infos = entity.Infos;
            Show = false;
        }

        public static List<ResponseTechInfos> ToListResponse(List<TechnologieInformation> list)
        {
            return list.Select(e =>  new ResponseTechInfos(e)).ToList();
        }
    }
}
