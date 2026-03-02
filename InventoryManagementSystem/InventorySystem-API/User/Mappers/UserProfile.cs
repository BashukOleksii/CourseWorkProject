using AutoMapper;
using InventorySystem_API.User.Model;
using InventorySystem_Shared.User;

namespace InventorySystem_API.User.Mapper
{
    public class UserProfile: Profile
    {
        public UserProfile() 
        {
            CreateMap<UserRegister, UserModel>();
            CreateMap<UserModel, UserResponse>();
        }
    }
}
