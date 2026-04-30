using InventorySystem_MAUI.Helper.Exceptions;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace InventorySystem_MAUI.Helper
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly IUserContextService _userContextService;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public AuthHandler(IUserContextService userContextService, IHttpClientFactory httpClientFactory)
        {
            _userContextService = userContextService;
            _httpClientFactory = httpClientFactory;
        }
        

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

                if (!string.IsNullOrEmpty(_userContextService.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userContextService.AccessToken);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshTokenAsync())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userContextService.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    _userContextService.LogOut();
                    throw new ApiException(System.Net.HttpStatusCode.Unauthorized, "Сесія вичерпана. Увійдіть знову.");
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                string errorMessage;

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content, options);

                    if (problemDetails?.Errors != null && problemDetails.Errors.Any())
                        errorMessage = string.Join("\n", problemDetails.Errors.SelectMany(x => x.Value));
                    else
                        errorMessage = !string.IsNullOrWhiteSpace(content) ? content : "Невідомий тип помилки";
                    
                }
                catch (JsonException)
                {
                    errorMessage = content;
                }

                throw new ApiException(response.StatusCode, errorMessage);
            }


            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                    throw new ApiEmptyResponseException();
            }

            return response;
        }

        private async Task<bool> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(_userContextService.AccessToken)
                || string.IsNullOrEmpty(_userContextService.RefreshToken)  ||
                _userContextService.CurrentUser is null)
                return false;

            try
            {
                var client = _httpClientFactory.CreateClient("AuthClient");
                var userRefresh = new UserRefresh
                {
                    UserId = _userContextService.CurrentUser.Id,
                    RefreshToken = _userContextService.RefreshToken
                };

                var response = await client.PostAsJsonAsync("api/auth/refresh", userRefresh);

                var tokens = await response.Content.ReadFromJsonAsync<TokensDataResponse>();

                await _userContextService.SetUserContextAsync(
                    _userContextService.CurrentUser,
                    tokens.AccessToken,
                    tokens.RefreshToken
                );

                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
