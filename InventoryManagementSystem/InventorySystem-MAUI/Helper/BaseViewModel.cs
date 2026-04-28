using CommunityToolkit.Mvvm.ComponentModel;
using InventorySystem_MAUI.Helper.Exceptions;


namespace InventorySystem_MAUI.Helper
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        protected async Task RunBusyTask(Func<Task> task)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await task();
            }
            catch (ApiException ex)
            {
                await ShellService.DisplayAlert("Помилка", ex.Content, "OK");
            }
            catch (ApiEmptyResponseException ex)
            {
                await ShellService.DisplayAlert("Помилка даних", ex.Message, "OK");
            }
            catch (HttpRequestException ex)
            {
                await ShellService.DisplayAlert("Мережа", "Не вдалося з'єднатися з сервером", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
