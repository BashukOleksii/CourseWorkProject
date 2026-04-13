using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.User;

namespace InventorySystem_MAUI.ViewModel;

[QueryProperty(nameof(UserId), "Id")]
[QueryProperty(nameof(SelectedWarehouseIds), "SelectedWarehouseIds")] 
public partial class UserDetailViewModel : BaseViewModel
{
    private readonly UserService _userService;

    [ObservableProperty] private string userId;
    [ObservableProperty] private UserResponse user;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WarehouseCountText))]
    private List<string> selectedWarehouseIds = new();

    public string WarehouseCountText => SelectedWarehouseIds?.Count > 0
        ? $"Призначено складів: {SelectedWarehouseIds.Count}"
        : "Склади не призначено";

    public bool IsManager => User?.UserRole == UserRole.manager;

    public UserDetailViewModel(UserService userService)
    {
        _userService = userService;
    }

    async partial void OnSelectedWarehouseIdsChanged(List<string> value)
    {
        if (value == null || User == null) return;

        await RunBusyTask(async () =>
        {
            await _userService.UpdateUserWarehouses(User.Id, value);
            await Shell.Current.DisplayAlertAsync("Успіх", "Список складів оновлено", "OK");
        });

        await LoadUserDetails();
    }

    public async Task LoadUserDetails()
    {
        await RunBusyTask(async () =>
        {
            User = await _userService.GetUserById(UserId);
            SelectedWarehouseIds = User.WarehouseIds ?? new List<string>();
        });
    }

    [RelayCommand]
    private async Task OpenWarehousePicker()
    {
        var parameters = new Dictionary<string, object>
        {
            { "InitialSelectedIds", SelectedWarehouseIds }
        };
        await ShellService.NavigateTo(nameof(View.WarehousePickerPage), parameters);
    }

    [RelayCommand]
    private async Task GoBack() => await ShellService.GoBack();
}