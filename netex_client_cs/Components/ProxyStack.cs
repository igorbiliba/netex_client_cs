using netex_client_cs.Data;
using netex_client_cs.Models;
using netex_client_cs.Netex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static netex_client_cs.Data.ProxySettings;

namespace netex_client_cs.Components
{
    public class ProxyStack
    {
        public EmailSender         emailSender;
        public ProxyStorageModelMixed proxyStorageModel;
        public ProxySettings       proxySettings;
        public NetexSettings       settings;

        int GetLastUsedId()
        {
            string lastUsedIp = proxyStorageModel.Get();
            return proxyStorageModel.Get() == null
                ? proxySettings.items.Length - 1
                : proxySettings
                    .items
                    .ToList()
                    .FindLastIndex(
                        el => el.ip.Trim().ToLower() == lastUsedIp.Trim().ToLower()
                    );
        }

        public ProxySettingsItem Next(ref List<string> usedProxyList, bool withCheckAccess = true, bool sendToEmailIfEmptyProxyList = true)
        {
            if (proxySettings.items.Count() == 0)
                return null;

            int lastUsedId = GetLastUsedId();
            if (lastUsedId >= proxySettings.items.Length - 1) lastUsedId = 0;
            else lastUsedId++;
            ProxySettingsItem proxy = proxySettings.items[lastUsedId];

            usedProxyList.Add(proxy.ip);
            proxyStorageModel.Update(proxy.ip);

            if (withCheckAccess)
            {
                Request request   = new Request() { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = proxy };
                NetexClient netex = new NetexClient() { request = request, settings = settings };

                if (!netex.TryLoadMainPage())
                {
                    proxySettings.RemoveFromFile(proxy);

                    if (sendToEmailIfEmptyProxyList && proxySettings.items.Count() == 0)
                        emailSender.Send("proxy list (" + proxySettings.file + ") is empty");

                    return Next(ref usedProxyList, withCheckAccess, sendToEmailIfEmptyProxyList);
                }
            }

            return proxy;
        }

        public ProxySettingsItem First(bool withCheckAccess = true, bool sendToEmailIfEmptyProxyList = true)
        {
            if (proxySettings.items.Count() == 0)
                throw new Exception("proxy list is empty");
            
            var proxy = proxySettings.items.First();

            if(withCheckAccess)
            {
                Request     request = new Request()     { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = proxy };
                NetexClient netex   = new NetexClient() { request = request, settings = settings };

                if(!netex.TryLoadMainPage())
                {
                    proxySettings.RemoveFromFile(proxy);

                    if (sendToEmailIfEmptyProxyList && proxySettings.items.Count() == 0)
                        emailSender.Send("proxy list (" + proxySettings.file + ") is empty");

                    return First(withCheckAccess, sendToEmailIfEmptyProxyList);
                }
            }

            return proxy;
        }
    }
}
