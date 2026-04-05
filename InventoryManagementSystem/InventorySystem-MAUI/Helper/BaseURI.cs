    using System;
    using System.Collections.Generic;
    using System.Text;

    namespace InventorySystem_MAUI.Helper
    {
        public static class Conection
        {
            public static string BaseURI =>
                DeviceInfo.Platform == DevicePlatform.Android ?
                    "http://10.0.0.2:5011/" :
                    "http://localhost:5011/";
        }
    }
