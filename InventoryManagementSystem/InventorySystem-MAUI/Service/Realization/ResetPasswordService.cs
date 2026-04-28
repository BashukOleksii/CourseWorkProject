using InventorySystem_Shared.User;
using System.Net.Http.Json;

namespace InventorySystem_MAUI.Service
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly HttpClient _httpClient;

        public ResetPasswordService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
        }

        public async Task RequestResetCode(string email) =>
            await _httpClient.GetAsync($"api/reset?email={email}");
        

        public async Task ConfirmResetPassword(string email, string code, string newPassword)
        {
            var dto = new ResetPasswordDTO { Email = email, Password = newPassword };
            var response = await _httpClient.PostAsJsonAsync($"api/reset?code={code}", dto);
        }
    }
}
