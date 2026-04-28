using InventorySystem_Shared.Loging;
using System.Net.Http.Json;

namespace InventorySystem_MAUI.Service
{
    public class LogService : ILogService
    {
        private readonly HttpClient _httpClient;

        public LogService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");
        }

        public async Task<List<AuditLogResponse>> GetLogs(AuditLogQuery query)
        {
            var parts = new List<string>
            {
                $"Page={query.Page}",
                $"PageSize={query.PageSize}"
            };

            if (!string.IsNullOrEmpty(query.UserId)) parts.Add($"UserId={query.UserId}");
            if (!string.IsNullOrEmpty(query.UserName)) parts.Add($"UserName={query.UserName}");
            if (query.Role.HasValue) parts.Add($"Role={query.Role}");
            if (query.Action.HasValue) parts.Add($"Action={query.Action}");
            if (!string.IsNullOrEmpty(query.EntityId)) parts.Add($"EntityId={query.EntityId}");
            if (query.EntityType.HasValue) parts.Add($"EntityType={query.EntityType}");

            if (query.From.HasValue) parts.Add($"From={query.From.Value:yyyy-MM-ddTHH:mm:ss}");
            if (query.To.HasValue) parts.Add($"To={query.To.Value:yyyy-MM-ddTHH:mm:ss}");

            var queryString = "?" + string.Join("&", parts);
            var response = await _httpClient.GetAsync($"api/AuditLog{queryString}");
            return await response.Content.ReadFromJsonAsync<List<AuditLogResponse>>();
        }
    }
}