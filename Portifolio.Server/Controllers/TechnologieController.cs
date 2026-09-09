using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.TechInfos;
using Portifolio.Server.Services.TechInfo_Services;

namespace Portifolio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnologieController : ControllerBase
    {
        private readonly ITechInfoService _tf;
        public TechnologieController(ITechInfoService tf)
        {
            _tf = tf;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _tf.Get());
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(TemplateTechInfo dto)
        {
            var result = await _tf.Post(dto);
            return result.StatusCode == 200 ? Ok() : BadRequest();
        }
        [HttpPut("id:{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id, TemplateTechInfo dto)
        {
            BaseResponse result = await _tf.Put(id, dto);
            return result.StatusCode switch
            {
                200 => Ok(),
                404 => NotFound(result.Message),
                400 => BadRequest(result.Message),
                _ => BadRequest()
            };
        }
    }
}
