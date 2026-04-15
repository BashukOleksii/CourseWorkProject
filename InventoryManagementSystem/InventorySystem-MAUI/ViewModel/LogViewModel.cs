using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventorySystem_MAUI.Helper;
using InventorySystem_MAUI.Service;
using InventorySystem_Shared.Loging;
using InventorySystem_Shared.User;
using System.Collections.ObjectModel;

public partial class LogViewModel : BaseViewModel
{
    private readonly LogService _logService;

    [ObservableProperty] private ObservableCollection<AuditLogResponse> logs = new();
    [ObservableProperty] private AuditLogQuery query = new();
    [ObservableProperty] private bool isFilterVisible;
    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private bool canGoNext;

    public List<UserRole?> RoleOptions { get; } = new() { null, UserRole.admin, UserRole.manager};
    public List<ActionType?> ActionOptions { get; } = Enum.GetValues(typeof(ActionType)).Cast<ActionType?>().Prepend(null).ToList();
    public List<EntityType?> EntityOptions { get; } = Enum.GetValues(typeof(EntityType)).Cast<EntityType?>().Prepend(null).ToList();

    public LogViewModel(LogService logService)
    {
        _logService = logService;
        Task.Run(async () => await ApplyFilters());
    }

    [RelayCommand]
    private void ToggleFilters() => IsFilterVisible = !IsFilterVisible;

    [RelayCommand]
    private void ResetFilters()
    {
        Query = new AuditLogQuery();
        CurrentPage = 1;
    }

    [RelayCommand]
    public async Task ApplyFilters()
    {
        await RunBusyTask(async () =>
        {
            Query.Page = CurrentPage;
            var result = await _logService.GetLogs(Query);
            Logs = new ObservableCollection<AuditLogResponse>(result);
            CanGoNext = result.Count == Query.PageSize;
            IsFilterVisible = false;
        });
    }

    [RelayCommand] private async Task NextPage() { CurrentPage++; await ApplyFilters(); }
    [RelayCommand] private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await ApplyFilters(); } }
}