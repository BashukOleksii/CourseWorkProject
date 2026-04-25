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
    public partial class UserReportViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        [ObservableProperty] private ObservableCollection<UserResponse> users = new();
        [ObservableProperty] private UserQuery query = new() { PageSize = 10, Page = 1 };
        [ObservableProperty] private bool isFilterVisible = false;

        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private bool canGoNext;

        public List<UserRole?> Roles { get; } = new() { null, UserRole.admin, UserRole.manager };

        public UserReportViewModel(UserService userService)
        {
            _userService = userService;
            Task.Run(async () => await ApplyFilters());
        }

        [RelayCommand]
        private void ToggleFilters() => IsFilterVisible = !IsFilterVisible;

        [RelayCommand]
        public async Task ApplyFilters()
        {
            await RunBusyTask(async () =>
            {
                Query.Page = CurrentPage;
                var result = await _userService.GetUsers(Query);

                Users = new ObservableCollection<UserResponse>(result);
                CanGoNext = result.Count == Query.PageSize;
                IsFilterVisible = false;
            });
        }

        [RelayCommand]
        private async Task GeneratePdfReport()
        {
            await RunBusyTask(async () =>
            {
                var pdfData = await _userService.GetUserReport(Query);
                if (pdfData == null) return;

                var fileName = $"User_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                await File.WriteAllBytesAsync(filePath, pdfData);

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath),
                    Title = "Звіт по персоналу"
                });
            });
        }

        [RelayCommand]
        private async Task EditUser(UserResponse user)
        {
            await ShellService.NavigateTo(nameof(UserDetailPage), new Dictionary<string, object> { { "Id", user.Id } });
        }

        [RelayCommand]
        private async Task DeleteUser(UserResponse user)
        {
            bool confirm = await Shell.Current.DisplayAlertAsync("Видалення", $"Видалити користувача {user.Name}?", "Так", "Ні");
            if (!confirm) return;

            await RunBusyTask(async () =>
            {
                await _userService.DeleteUser(user.Id);
                Users.Remove(user);
            });
        }

        [RelayCommand]
        private async Task NextPage() { CurrentPage++; await ApplyFilters(); }

        [RelayCommand]
        private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await ApplyFilters(); } }
    }
}
