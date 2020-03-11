using netex_client_cs.Components;
using netex_client_cs.Data;
using netex_client_cs.Models;
using netex_client_cs.Netex;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static netex_client_cs.Data.ProxySettings;

namespace netex_client_cs.Tests
{
    [TestFixture]
    class SettingsTest
    {
        NetexSettings settings;
        NetexClient netex;

        [SetUp]
        public void SettingsTestInit()
        {

            //settings = new Settings();
            //DB db = new DB();
            //UsedProxyModel usedProxyModel = new UsedProxyModel()
            //{
            //    db = db,
            //    settings = settings
            //};

            //ProxySettingsItem freeProxy =
            //    settings.proxy.items.Length == 0 ?
            //    null :
            //    usedProxyModel.FindFreeProxy(
            //        new ProxyLog()
            //            .Load()
            //            .GetBlacklistHosts()
            //    );
            //Request request = new Request() { BASE_URL = "https://api.netex24.net/", settings = settings, proxy = freeProxy };
            //netex = new NetexClient() { request = request, settings = settings };
        }

        [Test]
        public void expireMinOneIpTest()
        {
            Assert.IsTrue(settings.expireMinOneIp > 0, "Не загрузился expireMinOneIp");            
        }

        //[Test]
        //public void targetCurrenciesIdsTest()
        //{
        //    Assert.IsTrue(settings.targetCurrenciesIds.Length > 0, "Не загрузился targetCurrenciesIds");
        //}
    }
}
