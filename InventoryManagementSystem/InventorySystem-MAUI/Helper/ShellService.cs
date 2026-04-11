using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper
{
    public static class ShellService
    {
        public static async Task NavigateTo(string route, Dictionary<string, object> paramters = null)
        {
            if(paramters is null)
                await Shell.Current.GoToAsync(route);
            else
                await Shell.Current.GoToAsync(route,paramters);

        }

        public static async Task GoBack(Dictionary<string, object> paramters = null)
        {
            if(paramters is null)
                await Shell.Current.GoToAsync("..");
            else
                await Shell.Current.GoToAsync("..", paramters);
        }

        public static async Task AbsoluteOpenPage(string route, Dictionary<string, object> paramters = null)
        {
            if (paramters is null)
                 await Shell.Current.GoToAsync($"//{route}");
            else
                await Shell.Current.GoToAsync($"//{route}",paramters);
        } 
    }
}
