using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Service
{
    public interface IResetPasswordService
    {
        Task RequestResetCode(string email);
        Task ConfirmResetPassword(string email, string code, string newPassword);
    }
}
