using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.Projects;
using Portifolio.Server.Models;

namespace Portifolio.Server.Services.Project_Services
{
    public interface IProjectService
    {
        public Task<BaseResponse<List<Project>>> Get();
        public Task<BaseResponse<Project>> Get(string idStr);
        public Task<BaseResponse<Project>> Create(TemplateProject dto);
        public Task<BaseResponse<Project>> Update(string idStr, TemplateProject dto);
    }
}
