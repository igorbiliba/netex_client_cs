using netex_client_cs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Data
{
    public class ProxySettings
    {
        public ProxyStorageModelMixed createProxyStorageModel = null;

        public class ProxySettingsItem
        {
            public string ip { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public int port { get; set; }

            public WebProxy CreateProxyClient()
            {
                WebProxy myProxy = new WebProxy(ip, port);

                if (username == "" || username == null) return myProxy;
                if (password == "" || password == null) return myProxy;

                myProxy.Credentials = new NetworkCredential(username, password);
                return myProxy;
            }
        }

        public ProxySettingsItem[] items;

        public void RemoveFromFile(ProxySettingsItem proxy, bool andSetBeforeInDb = true)
        {
            var oldItems = items.ToList();

            List<ProxySettingsItem> list = new List<ProxySettingsItem>();

            foreach (var item in items)
            {
                if (item.ip.Trim().ToLower() == proxy.ip.Trim().ToLower())
                    continue;

                list.Add(item);
            }

            items = list.ToArray();
            if(SaveItemsToFile() && andSetBeforeInDb && createProxyStorageModel != null)
            {
                try
                {
                    int removeId = oldItems.IndexOf(proxy);
                    int beforeId = removeId == 0 ? oldItems.Count - 1 : removeId - 1;
                    var lastUsed = oldItems[beforeId];

                    createProxyStorageModel.Update(lastUsed.ip);
                }
                catch (Exception) { }
            }
        }

        public string file;
        public ProxySettings(string file)
        {
            this.file = file;
        }

        string PATH
        {
            get
            {
                if (Program.TEST && File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ProxyTest.json"))
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ProxyTest.json";

                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\"+ file;
            }
        }

        public bool SaveItemsToFile()
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(PATH, false))
                {
                    file.WriteLine(
                        JsonConvert.SerializeObject(
                            items
                        )
                    );
                }

                return true;
            }
            catch (Exception ex) { }

            return false;
        }

        public ProxySettings LoadSettings()
        {
            this.items = JsonConvert.DeserializeObject<ProxySettingsItem[]>(
                File.ReadAllText(PATH)
            );

            return this;
        }
    }
}
