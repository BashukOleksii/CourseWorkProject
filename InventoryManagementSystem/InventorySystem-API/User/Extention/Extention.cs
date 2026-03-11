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
    }
}
