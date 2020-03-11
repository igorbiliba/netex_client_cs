using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Models
{
    public class StorageModelREG
    {
        public const string STORAGE_KEY = "HOMER_STORAGE";

        public void Set(string key, string value)
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey helloKey = currentUserKey.CreateSubKey(STORAGE_KEY);
            helloKey.SetValue(key, value);
            helloKey.Close();
        }

        public string Get(string key)
        {
            try
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey helloKey = currentUserKey.CreateSubKey(STORAGE_KEY);
                var value = helloKey.GetValue(key, "");
                helloKey.Close();

                return value.ToString();
            }
            catch (Exception) { }

            return "";
        }
    }
}
