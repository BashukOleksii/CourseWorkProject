using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_MAUI.View;
using InventorySystem_Shared.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace InventorySystem_MAUI.ViewModel
{
    public partial class UserListViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private CancellationTokenSource _searchCts;

        [ObservableProperty] private ObservableCollection<UserResponse> users = new();
        [ObservableProperty] private string searchText;
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int pageSize = 10;
        [ObservableProperty] private bool canGoNext;

        public UserListViewModel(UserService userService)
        {
            _userService = userService;
        }

        async partial void OnSearchTextChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, _searchCts.Token);
                CurrentPage = 1;
                await LoadUsers();
            }
            catch (TaskCanceledException) { }
        }

        [RelayCommand]
        public async Task LoadUsers()
        {
            await RunBusyTask(async () =>
            {
                var query = new UserQuery { Name = SearchText, Page = CurrentPage, PageSize = PageSize };
                var result = await _userService.GetUsers(query);
                Users = new ObservableCollection<UserResponse>(result);
                CanGoNext = result.Count == PageSize;
            });
        }

        [RelayCommand]
        private async Task AddUser()
        {
            await ShellService.NavigateTo(nameof(UserCreateFromAdminPage));
        }

        [RelayCommand]
        private async Task EditUser(UserResponse user)
        {
            await ShellService.NavigateTo(nameof(UserDetailPage), new Dictionary<string, object>
            {
                { "Id", user.Id }
            });
        }

        [RelayCommand]
        private async Task DeleteUser(UserResponse user)
        {
            if (await Shell.Current.DisplayAlertAsync("Видалення", $"Видалити {user.Name}?", "Так", "Ні"))
            {
                await RunBusyTask(async () => {
                    await _userService.DeleteUser(user.Id);
                    Users.Remove(user);
                });
            }
        }
    }
}
