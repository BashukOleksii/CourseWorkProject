using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace InventorySystem_MAUI.Helper
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly UserContextService _userContextService;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public AuthHandler(UserContextService userContextService, IHttpClientFactory httpClientFactory)
        {
            _userContextService = userContextService;
            _httpClientFactory = httpClientFactory;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(!string.IsNullOrEmpty(_userContextService.AccessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userContextService.AccessToken);

            var response = await base.SendAsync(request, cancellationToken);

            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if(await RefreshTokenAsync())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userContextService.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    _userContextService.LogOut();
                }
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

                var response = await client.PostAsJsonAsync("auth/refresh", userRefresh);

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
