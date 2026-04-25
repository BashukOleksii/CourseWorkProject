using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem_MAUI.Helper
{
    public class ValidationProblemDetails
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
