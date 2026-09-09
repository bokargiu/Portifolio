using Microsoft.AspNetCore.Mvc;
using Portifolio.Server.DTOs;
using Portifolio.Server.DTOs.TechInfos;

namespace Portifolio.Server.Services.TechInfo_Services
{
    public interface ITechInfoService
    {
        public Task<List<ResponseTechInfos>> Get();
        public Task<BaseResponse> Post(TemplateTechInfo dto);
        public Task<BaseResponse> Put(string id, TemplateTechInfo dto);
    }
}
