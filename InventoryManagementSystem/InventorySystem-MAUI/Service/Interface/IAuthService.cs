using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public interface IAuthService
    {
        Task Login(string email, string password);
        Task Register(UserRegister userRegister, FileResult? photo);
    }
}
