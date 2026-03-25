using AutoMapper;
using InventorySystem_API.Loging.Models;
using InventorySystem_Shared.Loging;

namespace InventorySystem_API.Loging.Mapper
{
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile() 
        {
            CreateMap<AuditLogCreate,AuditLogModel>().ReverseMap();
            CreateMap<AuditLogModel, AuditLogResponse>();
        }
    }
}
