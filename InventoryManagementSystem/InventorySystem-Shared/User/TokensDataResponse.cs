using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_Shared.User
{
    public class TokensDataResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public int AccessTokenExpiresMinuts { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public int RefreshTokenExpireDays { get; set; }
    }
}
