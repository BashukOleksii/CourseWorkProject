using AutoMapper;
using InventorySystem_API.Inventory.Models;
using InventorySystem_Shared.Inventory;

namespace InventorySystem_API.Inventory.Mapper
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile() 
        {
            CreateMap<InventoryCreate, InventoryModel>().ReverseMap();
            CreateMap<InventoryModel, InventoryResponse>().ReverseMap();
        }
    }
}
