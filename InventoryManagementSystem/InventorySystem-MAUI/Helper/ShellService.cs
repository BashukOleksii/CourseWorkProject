using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper
{
    public static class ShellService
    {
        public static async Task NavigateTo(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public static async Task AbsoluteOpenPage(string route)
        {
            await Shell.Current.GoToAsync($"//{route}");
        } 
    }
}
