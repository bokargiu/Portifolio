using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Server.DTOs.Projects;
using Portifolio.Server.DTOs.TechInfos;
using Portifolio.Server.Services.Project_Services;

namespace Portifolio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService service;
        public ProjectController(IProjectService service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await service.Get();
            return StatusCode(response.StatusCode, response.Data);
        }
        [HttpGet("id:{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string id)
        {
            var response = await service.Get(id);
            return StatusCode(response.StatusCode, response.StatusCode == 200 ? response.Data : response.Message);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] TemplateProject dto)
        {
            var response = await service.Create(dto);
            return StatusCode(response.StatusCode, response.StatusCode == 201 ? response.Data : response.Message);
        }
        [HttpPost("Range")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(List<TemplateProject> dtos)
        {
            List<string> strings = new List<string>();
            foreach (var dto in dtos)
            {
                var result = await service.Create(dto);
                strings.Add($"{result.StatusCode} | {result.Message}\n");
            }
            return Ok(strings);
        }
        [HttpPut("id:{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] TemplateProject dto)
        {
            var response = await service.Update(id, dto);
            return StatusCode(response.StatusCode, response.StatusCode == 200 ? response.Data : response.Message);
        }
    }
}
