using AutoMapper;
using InventorySystem_API.Warehouse.Models;
using InventorySystem_Shared.Warehouse;

namespace InventorySystem_API.Warehouse.Mapper
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile() 
        {
            CreateMap<WarehouseDTO, WarehouseModel>().ReverseMap();
            CreateMap<WarehouseModel, WarehouseResponse>();
        }
    }
}
