using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string Content { get; }

        public ApiException(int statusCode, string content)
        {
            StatusCode = statusCode;
            Content = content;
        }


        public static async Task ShowException(HttpResponseMessage? message)
        {
            var messageContent = await message.Content.ReadAsStringAsync();
            throw new ApiException((int)message.StatusCode, messageContent);
        }
    }
}

