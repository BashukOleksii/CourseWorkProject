using InventorySystem_API.User.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InventorySystem_API.User.Services
{
    public class TokenGenerator
    {
        private readonly JWTSettingOptions _jWTSettingOptions;
        public TokenGenerator(IOptions<JWTSettingOptions> options)  =>
            _jWTSettingOptions = options.Value;
        
        public string GenerateAccessToken(UserModel userModel)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettingOptions.SecretKey));
            var signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, userModel.Id),
                new Claim(ClaimTypes.GroupSid, userModel.CompanyId),
                new Claim(ClaimTypes.Name, userModel.Name),
                new Claim(ClaimTypes.Email, userModel.Email),
                new Claim(ClaimTypes.Role, userModel.UserRole.ToString()),
            };

            if (userModel.WarehouseIds is not null)
                claims.AddRange(userModel.WarehouseIds
                    .Select(id => new Claim("WarehouseId", id))
                );

            var token = new JwtSecurityToken(
                issuer: _jWTSettingOptions.Issuer,
                audience: _jWTSettingOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jWTSettingOptions.ExpiryInMinutes),
                signingCredentials: signingCreds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }

        public RefeshToken GenerateRefreshToken()
        {
            var bytes = new byte[32];
            var rndGenerator = RandomNumberGenerator.Create();
            rndGenerator.GetBytes(bytes);

            return new RefeshToken
            {
                RefreshToken = Convert.ToBase64String(bytes),
                ExpiryDate = DateTime.UtcNow.AddDays(_jWTSettingOptions.RefreshTokenExpiryInDays)
            };
        }
    }
}
