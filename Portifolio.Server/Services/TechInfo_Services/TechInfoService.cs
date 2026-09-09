using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portifolio.Server.Database;
using Portifolio.Server.DTOs.TechInfos;
using Portifolio.Server.DTOs;
using Portifolio.Server.Models;

namespace Portifolio.Server.Services.TechInfo_Services
{
    public class TechInfoService : ITechInfoService
    {
        private readonly Database.DB _context;
        public TechInfoService(Database.DB context)
        {
            _context = context;
        }

        public async Task<List<ResponseTechInfos>> Get()
        {
            var entitys = await _context.Technologies.ToListAsync();
            return ResponseTechInfos.ToListResponse(entitys.OrderBy(e => e.Position).ToList());
        }
        public async Task<BaseResponse> Post(TemplateTechInfo dto)
        {
            if (String.IsNullOrEmpty(dto.Title) ||
                String.IsNullOrEmpty(dto.Infos) ||
                String.IsNullOrEmpty(dto.IconName))
                return new BaseResponse(400, message: "Invalid input data");

            TechnologieInformation tf = new TechnologieInformation();
            tf.Title = dto.Title;
            tf.Infos = dto.Infos;
            tf.IconName = dto.IconName;
            tf.Position = dto.Position;

            await _context.Technologies.AddAsync(tf);
            await _context.SaveChangesAsync();
            return new BaseResponse(200);
        }
        public async Task<BaseResponse> Put(string id, TemplateTechInfo dto)
        {
            if (!Guid.TryParse(id, out Guid technologyId))
                return new BaseResponse(400, message: "Incorrect Id format.");

            var entity = await _context.Technologies.FirstOrDefaultAsync(t => t.Id == technologyId);
            if (entity == null)
                return new BaseResponse(404, message: "Technology not found.");
            
            if(!String.IsNullOrEmpty(dto.Title) && dto.Title != entity.Title)
                entity.Title = dto.Title;
            if(!String.IsNullOrEmpty(dto.Infos) && dto.Infos != entity.Infos)
                entity.Infos = dto.Infos;
            if(!String.IsNullOrEmpty(dto.IconName) && dto.IconName != entity.IconName)
                entity.IconName = dto.IconName;
            if(dto.Position != entity.Position && dto.Position > 0)
                entity.Position = dto.Position;

            await _context.SaveChangesAsync();
            return new BaseResponse(200);
        }
    }
}
