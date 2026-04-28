using InventorySystem_MAUI.Helper;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public interface IUserService
    {
        Task<UserResponse> UpdateUser(string id, UserUpdate update, FileResult photo = null);

        Task<List<UserResponse>> GetUsers(UserQuery query);

        Task DeleteUser(string id);

        Task UpdateUserWarehouses(string userId, List<string> warehouseIds);
        Task<UserResponse> GetUserById(string id);

        Task<byte[]> GetUserReport(UserQuery query);
    }
}
