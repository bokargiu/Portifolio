using Microsoft.EntityFrameworkCore;
using Portifolio.Server.Database;
using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.Projects;
using Portifolio.Server.Models;

namespace Portifolio.Server.Services.Project_Services
{
    public class ProjectService : IProjectService
    {
        private readonly DB _context;
        public ProjectService(DB context)
        {
            _context = context;
        }

        public async Task<BaseResponse<List<Project>>> Get()
        {
            return new BaseResponse<List<Project>>(200, data: await _context.Projects.ToListAsync());
        }
        public async Task<BaseResponse<Project>> Get(string idStr)
        {
            if (string.IsNullOrEmpty(idStr) || Guid.TryParse(idStr, out Guid id))
                return new BaseResponse<Project>(400, message: "Invalid id");
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            return project is null ?
                        new BaseResponse<Project>(404, message: "Project not found")
                        : new BaseResponse<Project>(200, data: project);
        }
        public async Task<BaseResponse<Project>> Create(TemplateProject dto)
        {
            if (dto is null 
                || string.IsNullOrEmpty(dto.Name) 
                || string.IsNullOrEmpty(dto.Description) 
                || dto.IconsTech is null)
                return new BaseResponse<Project>(400, message: "Project is null");
            var project = new Project(dto);

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return new BaseResponse<Project>(201, data: project);
        }
        public async Task<BaseResponse<Project>> Update(string idStr, TemplateProject dto)
        {
            if (dto is null 
                || string.IsNullOrEmpty(dto.Name) 
                || string.IsNullOrEmpty(dto.Description) 
                || dto.IconsTech is null)
                return new BaseResponse<Project>(400, message: "Project is null");

            var baseProject = await Get(idStr);
            if (baseProject.StatusCode != 200)
                return baseProject;

            Project project = new Project(dto);
            project.Id = baseProject.Data!.Id;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return new BaseResponse<Project>(200, data: project);
        }
    }
}
