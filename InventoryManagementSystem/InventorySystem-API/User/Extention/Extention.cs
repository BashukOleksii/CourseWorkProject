using InventorySystem_Shared.User;
using System.Security.Claims;

namespace InventorySystem_API.User.Extention
{
    public static class Extention
    {
        public static string GetCompanyId(this ClaimsPrincipal claimsPrincipal)
        {
            var companyId = claimsPrincipal.FindFirstValue(ClaimTypes.GroupSid);

            if (companyId == null)
                throw new ArgumentNullException("Немає id компанії");

            return companyId;
        }

        public static string GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var value = claimsPrincipal.FindFirstValue(ClaimTypes.Sid);

            if (value == null)
                throw new ArgumentNullException("Немає id");

            return value;
        }

        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            var value = claimsPrincipal.FindFirstValue(ClaimTypes.Name);

            if (value == null)
                throw new ArgumentNullException("Немає імені");

            return value;
        }

        public static string GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            var value = claimsPrincipal.FindFirstValue(ClaimTypes.Role);

            if (value == null)
                throw new ArgumentNullException("Немає ролі");

            return value;
        }
    }
}
