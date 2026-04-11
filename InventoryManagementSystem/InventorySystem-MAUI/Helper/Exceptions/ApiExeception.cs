using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper.Exceptions
{
    public class ApiEmptyResponseException : Exception
    {
        public ApiEmptyResponseException() : base("Сервер повернув порожню відповідь.") { }
    }

    public class ApiException : Exception
    {
        public System.Net.HttpStatusCode StatusCode { get; }
        public string Content { get; }

        public ApiException(System.Net.HttpStatusCode statusCode, string content)
            : base($"Помилка API: {statusCode}")
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}

